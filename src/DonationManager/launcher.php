<?php
require_once 'assign_badge.php';
require_once 'assign_badge_initializer.php';
require_once 'load_spreadsheet.php';
require_once 'models/badges.php';
require_once 'db/db_conn.php';

// Load spreadsheet data
$donators = LoadSpreadsheet::loadData();

// Connect to DB
$conn = DbConnection::Connect();

// Update donator DB
// TODO

// Request badgr.io api token
//$init = new AssignBadgeInitializer();
//if($init->authToken == null || empty($init->authToken)) {
//    echo "no token provided." . PHP_EOL;
//    return;
//}

// Get list of users that have to receive association badge
$associateUsers = DbConnection::GetContributorsWithoutAssociationBadge($conn);

// Get list of users that haven't received a badge yet
// TODO



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



// Assign badges
//$assignBadge = new AssignBadge($init->authToken);

// TODO
//$result = $assignBadge->issueBadge("b_paolini@yahoo.com", "http://codemooc.net", "s3CCyDURTe6HYyEjz88XBw");
