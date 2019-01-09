<?php
require_once 'assign_badge.php';
require_once 'assign_badge_initializer.php';

// Load spreadsheet data
LoadSpreadsheet::loadData();

// Cross-check users that need to receive token
// TODO

// Request badgr.io api token
$init = new AssignBadgeInitializer();
if($init->authToken == null || empty($init->authToken)) {
    echo "no token provided." . PHP_EOL;
    return;
}

// Assign badges
$assignBadge = new AssignBadge($init->authToken);

// TODO
//$result = $assignBadge->issueBadge("b_paolini@yahoo.com", "http://codemooc.net", "s3CCyDURTe6HYyEjz88XBw");
