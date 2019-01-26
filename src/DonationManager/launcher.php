<?php
require_once 'assign_badge.php';
require_once 'load_spreadsheet.php';
require_once 'models/badges.php';
require_once 'db_conn.php';
require_once 'salty_hasher.php';

$generated_badge_list = array();

// Init date timezione
date_default_timezone_set('UTC');

echo "Starting donation processing now" . PHP_EOL;

// Load spreadsheet data
$donators = LoadSpreadsheet::loadData();
echo "Loaded " . count($donators) . " total donations" . PHP_EOL;

// Start DB connection
$conn = DbConnection::Connect();
if(!$conn) {
    die("Cannot connect to database");
}

// Request badgr.io api token
$assignBadge = new AssignBadge();
if($assignBadge->isDryRun()) {
    echo "PERFORMING DRY RUN (will not assign badges)" . PHP_EOL;
}

// Update donor DB
echo "Updating donor DB... ";
$donorUpsertCount = 0;
foreach ($donators as $donator) {
    DbConnection::InsertDonator($conn, $donator);
    $donorUpsertCount++;
}
echo "$donorUpsertCount processed." . PHP_EOL;

// Get list of users that have to receive association badge
$associateUsers = DbConnection::GetContributorsWithoutAssociationBadge($conn);
echo "Found " . count($associateUsers) . " donors without association badge..." . PHP_EOL;

foreach($associateUsers as $email)
{
    $badgeType = Badges::ISCRIZIONE_2019;
    launchBadgeAssigner($assignBadge, $conn, $email, $badgeType);
}
echo "...done." . PHP_EOL;

// Get all donations
$donors = DbConnection::GetAllContributorsForYear($conn, "2019");
if($donors === false)
{
    die("Couldn't get donor list");
    return;
}

echo "Processing " . count($donors) . " donors for badges..." . PHP_EOL;
foreach ($donors as $donor)
{
    // Get donation type
    $amount = $donor['Amount'];
    $donorEmail = $donor['Email'];
    
    if($amount < 50) {
        continue;
    }
    $badgeType = Badges::SOSTENITORE_2019;
    if($amount >= 100) {
        $badgeType = Badges::SOSTENITORE_GOLD_2019;
    }
    else if($amount >= 1000) {
        $badgeType = Badges::DONATORE_SPONSOR_2019;
    }

    $query = DbConnection::GetContributorBadges($conn, $donorEmail, Badges::badgeList()[$badgeType]['name']);
    if($query === false)
    {
        continue;
    }

    launchBadgeAssigner($assignBadge, $conn, $donorEmail, $badgeType);
}
echo "...done." . PHP_EOL;

echo "Safety check for badges to retract... ";
$retractList = DbConnection::GetBadgesToRetract($conn);
echo count($retractList) . " to delete found." . PHP_EOL;
foreach($retractList as $retract)
{
    echo $retract[0] . ", " . $retract[1] . PHP_EOL;
}

echo "Sending summary e-mail... ";
$summary_mail_to = getenv('CONFIRMATION_MAIL_BCC');
if(!empty($summary_mail_to)) {
    $summary_mail_text =  "<html><body><p>Hello,<br />this is your friendly donation manager for CODEMOOC NET. 🤖</p>";
    $summary_mail_text .= "<p>Summary for <b>" . date('j F Y') . "</b>.</p>";
    $summary_mail_text .= "<p>$donorUpsertCount unique donors.</p>";
    if(count($generated_badge_list) > 0) {
        $summary_mail_text .= "<p>Generated " . count($generated_badge_list) . " badges:<br />";
        foreach($generated_badge_list as $badge) {
            $summary_mail_text .= $badge[0] . " => " . $badge[1] . "<br />";
        }
        $summary_mail_text .= "</p>";
    }
    else {
        $summary_mail_text .= "<p>No badges generated today.</p>";
    }
    $summary_mail_text .= "<p>See you tomorrow. 👋</p></body></html>";

    $transport = (new Swift_SmtpTransport(getenv('SMTP_HOST'), getenv('SMTP_PORT')))
        ->setUsername(getenv('SMTP_USERNAME'))
        ->setPassword(getenv('SMTP_PASSWORD'))
    ;
    $mailer = new Swift_Mailer($transport);
    
    $message = (new Swift_Message('🏅 Badge generation summary'))
        ->setFrom(['no-reply@codemooc.net' => 'CODEMOOC NET'])
        ->setTo([$summary_mail_to])
        ->setContentType('text/html')
        ->setBody($summary_mail_text)
        ;
    
    // Send the message
    $result = $mailer->send($message);
    if($result !== 1) {
        echo "result is $result! ";
    }
}

echo "done." . PHP_EOL;

echo "All done." . PHP_EOL;

/**
 * @param $assignBadge AssignBadge
 * @param $conn mysqli
 * @param $email
 * @param $badgeType
 */
function launchBadgeAssigner($assignBadge, $conn, $email, $badgeType)
{
    global $generated_badge_list;

    $token = SaltyHasher::hash();
    $urlPath = "";
    switch ($badgeType)
    {
        case Badges::ISCRIZIONE_2019:
            $urlPath = "socio2019";
            break;
        case Badges::SOSTENITORE_2019:
            $urlPath = "sostenitore2019";
            break;
        case Badges::SOSTENITORE_GOLD_2019:
            $urlPath = "sostenitoregold2019";
            break;
        case Badges::DONATORE_SPONSOR_2019:
            $urlPath = "sponsor2019";
            break;
    }
    if($token === false) {
        echo "Error generating hash for $email :/" . PHP_EOL;
        return;
    }

    $evidence = "https://codemooc.net/badge/$urlPath/evidence/$token";
    $result = $assignBadge->issueBadge($email, $evidence, $badgeType);
    if($result === false) {
        echo "Error assigning " . Badges::badgeList()[$badgeType]["name"] . " to $email" . PHP_EOL;
        return;
    }

    echo "Issued badge " . Badges::badgeList()[$badgeType]["name"] . " to $email." . PHP_EOL;
    // Update Badges table in DB
    $res = DbConnection::InsertAssignedBadge($conn, $email, $token, Badges::badgeList()[$badgeType]["name"]);
    if($res === false)
    {
        echo "Error inserting in DB " . Badges::badgeList()[$badgeType]["name"] . $email . PHP_EOL;
        return;
    }

    $generated_badge_list[] = array(
        $email, Badges::badgeList()[$badgeType]["name"]
    );
}
