<?php
require_once 'assign_badge.php';
require_once 'load_spreadsheet.php';
require_once 'models/badges.php';
require_once 'db_conn.php';
require_once 'salty_hasher.php';

$glob_log = "";
function append_to_log($str) {
    global $glob_log;

    if($str) {
        $glob_log .= ($str . "\n");
    }
    echo $str . PHP_EOL;
}

$generated_badge_list = array();

// Init date timezione
date_default_timezone_set('UTC');

append_to_log("Starting donation processing now");

// Load spreadsheet data
$donators = LoadSpreadsheet::loadData();
append_to_log("Loaded " . count($donators) . " total donations");

// Start DB connection
$conn = DbConnection::Connect();
if(!$conn) {
    die("Cannot connect to database");
}

// Request badgr.io api token
$assignBadge = new AssignBadge();
if($assignBadge->isDryRun()) {
    append_to_log("PERFORMING DRY RUN (will not assign badges)");
}

// Update donor DB
append_to_log("Updating donor DB... ");
$donorUpsertCount = 0;
foreach ($donators as $donator) {
    DbConnection::InsertDonator($conn, $donator);
    $donorUpsertCount++;
}
append_to_log("$donorUpsertCount processed.");

// Get list of users that have to receive association badge
/*$associateUsers = DbConnection::GetContributorsWithoutAssociationBadge($conn);
echo "Found " . count($associateUsers) . " donors without association badge..." . PHP_EOL;

foreach($associateUsers as $email)
{
    $badgeType = Badges::ISCRIZIONE_2019;
    launchBadgeAssigner($assignBadge, $conn, $email, $badgeType);
}
echo "...done." . PHP_EOL;*/

// Get all donations
$donors = DbConnection::GetAllContributorsForYear($conn, '2020');
if($donors === false)
{
    die("Couldn't get donor list");
    return;
}

append_to_log("Processing " . count($donors) . " donors for badges...");
foreach ($donors as $donor)
{
    // Get donation type
    $amount = $donor['Amount'];
    $donorEmail = $donor['Email'];
    $badges = DbConnection::GetContributorBadgesForYear($conn, $donorEmail, '2020');

    if($amount < 20) {
        if(count($badges) > 0) {
            append_to_log("User $donorEmail has badges with insufficient donation!");
        }
        continue;
    }

    if(!in_array('Iscrizione', $badges)) {
        launchBadgeAssigner($assignBadge, $conn, $donorEmail, 'Iscrizione', 'socio/2020', 'y0KPBhtPT5K6DH8vpxLCNg');
    }

    if($amount >= 1000 && !in_array('DonatoreSponsor', $badges)) {
        launchBadgeAssigner($assignBadge, $conn, $donorEmail, 'DonatoreSponsor', 'sponsor/2020', 'ubQoQa8eSXWRV0R_eLN7wA');
    }
    else if($amount >= 100 && !in_array('SostenitoreGold', $badges)) {
        launchBadgeAssigner($assignBadge, $conn, $donorEmail, 'SostenitoreGold', 'sostenitoregold/2020', 'X5ZR6AmsTQ-VdshdLPirfQ');
    }
    else if($amount >= 50 && !in_array('Sostenitore', $badges)) {
        launchBadgeAssigner($assignBadge, $conn, $donorEmail, 'Sostenitore', 'sostenitore/2020', 'Vx2oxvqiTXCSfnGlTlRDRw');
    }
}
append_to_log("...done.");

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
    $summary_mail_text .= "<p>See you tomorrow. 👋</p>";

    $summary_mail_text .= "<p><b>Full log:</b><br />" . nl2br($glob_log) . "</p>";

    $summary_mail_text .= "</body></html>";

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
        echo "result: $result! ";
    }
}
echo "done." . PHP_EOL;

append_to_log("All done.");

/**
 * @param $assignBadge AssignBadge
 * @param $conn mysqli
 * @param $email
 * @param $badgeType
 */
function launchBadgeAssigner($assignBadge, $conn, $email, $badgeName, $evidenceUrlPath, $badgrBadgeID)
{
    global $generated_badge_list;

    $token = SaltyHasher::hash();
    $evidence = "https://codemooc.net/badge/$evidenceUrlPath/evidence/$token";
    $result = $assignBadge->issueBadge($email, $evidence, $badgrBadgeID);
    if($result === false) {
        append_to_log("Error assigning $badgeName to $email");
        return;
    }

    append_to_log("Issued badge $badgeName to $email");

    if(!$assignBadge->isDryRun()) {
        // Update Badges table in DB
        $res = DbConnection::InsertAssignedBadge($conn, $email, $token, $badgeName);
        if($res === false) {
            append_to_log("Error inserting $badgeName in DB for $email");
            return;
        }
    }

    $generated_badge_list[] = array(
        $email, $badgeName
    );
}
