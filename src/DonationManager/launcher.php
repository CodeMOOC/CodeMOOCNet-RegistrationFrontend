<?php
require_once 'assign_badge.php';
require_once 'assign_badge_initializer.php';
require_once 'load_spreadsheet.php';
require_once 'models/badges.php';
require_once 'db_conn.php';
require_once 'salty_hasher.php';

// Init date timezione
date_default_timezone_set('UTC');

echo "Starting donation processing now" . PHP_EOL;

// Load spreadsheet data
$donators = LoadSpreadsheet::loadData();
echo "Loaded " . count($donators) . " donors" . PHP_EOL;

// Start DB connection
$conn = DbConnection::Connect();
if(!$conn)
    return;

// Request badgr.io api token
$init = new AssignBadgeInitializer();
if($init->authToken == null || empty($init->authToken)) {
    echo "no token provided." . PHP_EOL;
    return;
}
$assignBadge = new AssignBadge($init->authToken);

// Update donator DB
foreach ($donators as $donator) {
    echo "upserting user " . $donator->email . "..." . PHP_EOL;
    DbConnection::InsertDonator($conn, $donator);
}

// Get list of users that have to receive association badge
$associateUsers = DbConnection::GetContributorsWithoutAssociationBadge($conn);
echo "Found " . count($associateUsers) . " donors without association badge" . PHP_EOL;


if(!$associateUsers)
    echo "No users to associate." . PHP_EOL;
else
{
    /***************************/
    /* Send association badges */
    /***************************/
    echo "Launching badge assigner for users..." . PHP_EOL;
    foreach($associateUsers as $email)
    {
        $badgeType = Badges::ISCRIZIONE_2019;
        launchBadgeAssigner($assignBadge, $conn, $email, $badgeType);
    }
}

// Get all donations
$donors = DbConnection::GetAllContributorsForYear($conn, "2019");
if($donors === false)
{
    echo "Couldn't get donors list. " . PHP_EOL;
    return;
}

foreach ($donors as $donor)
{
    echo "Checking badges for " . $donor['Email'] . ", amount: " . $donor['Amount'] . PHP_EOL;

    // Get donation type
    $amount = $donor['Amount'];
    $donorEmail = $donor['Email'];
    $badgeType = "";
    if($amount >= 50 && $amount < 100)
        $badgeType = Badges::SOSTENITORE_2019;
    else if ($amount >= 100 && $amount < 1000)
        $badgeType = Badges::SOSTENITORE_GOLD_2019;
    else if ($amount >= 1000)
        $badgeType = Badges::DONATORE_SPONSOR_2019;

    // Assign badge
    if(empty($badgeType) || $badgeType == "" || $badgeType == null)
    {
        echo "User can't receive a badge. " . PHP_EOL;
        continue;
    }

    $query = DbConnection::GetContributorBadges($conn, $donorEmail, Badges::badgeList()[$badgeType]['name']);
    if($query === false)
    {
        echo "User Badge already sent." . PHP_EOL;
        continue;
    }

    /************************/
    /* Send donation badges */
    /************************/
    echo "Assigning " . Badges::badgeList()[$badgeType]['name'] . " badge to $donorEmail" . PHP_EOL;
    launchBadgeAssigner($assignBadge, $conn, $donorEmail, $badgeType);
}

/**
 * @param $assignBadge AssignBadge
 * @param $conn mysqli
 * @param $email
 * @param $badgeType
 */
function launchBadgeAssigner($assignBadge, $conn, $email, $badgeType)
{
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

    // TODO: switch path for production
    $evidence = "https://botify.it/codemoocnetbeta/badge/$urlPath/evidence/$token";
    $result = $assignBadge->issueBadge($email, $evidence, $badgeType);
    if($result === false) {
        echo "Error assigning " . Badges::badgeList()[$badgeType]["name"] . " - " . $badgeType . "to $email" . PHP_EOL;
        return;
    }

    echo "Issued badge for $email, inserting user in DB..." . PHP_EOL;
    // Update Badges table in DB
    $res = DbConnection::InsertAssignedBadge($conn, $email, $token, Badges::badgeList()[$badgeType]["name"]);
    if($res === false)
    {
        echo "Error inserting in DB " . Badges::badgeList()[$badgeType]["name"] . $email . PHP_EOL;
        return;
    }
}
