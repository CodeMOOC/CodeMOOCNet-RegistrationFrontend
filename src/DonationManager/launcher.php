<?php
require_once 'assign_badge.php';
require_once 'assign_badge_initializer.php';
require_once 'load_spreadsheet.php';
require_once 'models/badges.php';

// Load spreadsheet data
$donators = LoadSpreadsheet::loadData();

// Update donator DB
// TODO

// Request badgr.io api token
//$init = new AssignBadgeInitializer();
//if($init->authToken == null || empty($init->authToken)) {
//    echo "no token provided." . PHP_EOL;
//    return;
//}

foreach(Badges::badgeList() as $badge => $type)
{
    if($type == Badges::IS_ASSOCIATION)
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
