<?php
require_once 'assign_badge.php';
require_once 'assign_badge_initializer.php';
require_once 'load_spreadsheet.php';
require_once 'models/badges.php';
require_once 'db_conn.php';

date_default_timezone_set('UTC');

echo "Starting donation processing now" . PHP_EOL;

// Load spreadsheet data
$donators = LoadSpreadsheet::loadData();
echo "Loaded " . count($donators) . " donors" . PHP_EOL;

// Start DB connection
$conn = DbConnection::Connect();
if(!$conn)
    return;

// Update donator DB
foreach ($donators as $donator) {
    echo "upserting user " . $donator->email . "..." . PHP_EOL;
    DbConnection::InsertDonator($conn, $donator);
}

// Get list of users that have to receive association badge
$associateUsers = DbConnection::GetContributorsWithoutAssociationBadge($conn);
var_dump($associateUsers);
if(!$associateUsers)
    echo "No users to associate." . PHP_EOL;

// Request badgr.io api token
//$init = new AssignBadgeInitializer();
//if($init->authToken == null || empty($init->authToken)) {
//    echo "no token provided." . PHP_EOL;
//    return;
//}

// Get list of users that haven't received a badge yet
// TODO


/*
foreach(Badges::badgeList() as $badge => $info)
{
    if($info["type"] == Badges::IS_ASSOCIATION)
    {
        // Assign association badge if necessary
        // TODO
    }
    else
    {
        // Assign contributor badge if necessary
        // TODO
    }
}
*/


// Assign badges
//$assignBadge = new AssignBadge($init->authToken);

// TODO
//$result = $assignBadge->issueBadge("b_paolini@yahoo.com", "http://codemooc.net", "s3CCyDURTe6HYyEjz88XBw");
