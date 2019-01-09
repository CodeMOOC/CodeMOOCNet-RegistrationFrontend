<?php
require 'vendor/autoload.php';
require_once 'auth.php';
require_once 'assign_badge.php';

class AssignBadgeInitializer
{
    public $client;
    public $authToken;

    public function __construct()
    {
        $cl = new GuzzleHttp\Client(['base_uri' => 'https://api.badgr.io/', 'verify' => false]);

        $authorization = new Authorization();
        $auth = $authorization->auth($cl);

        if ($auth["success"])
        {
            $this->client = $cl;
            $this->authToken = $auth["token"];
        }
        else
            echo $auth["message"];
    }
}