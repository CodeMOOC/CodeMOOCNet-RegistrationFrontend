<?php
require_once 'assign_badge.php';
require_once 'load_spreadsheet.php';
require_once 'models/badges.php';
require_once 'db_conn.php';
require_once 'salty_hasher.php';

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
}
