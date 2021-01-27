<?php
require(dirname(__FILE__) . '/vendor/autoload.php');
require_once(dirname(__FILE__) . '/auth.php');

class AssignBadge
{
    /**
     * @var string
     */
    public $authToken;

    public function isDryRun() {
        return ($this->authToken == null || empty($this->authToken));
    }

    /**
     * AssignBadge constructor.
     * @param $client GuzzleHttp\Client
     * @param $token string
     */
    public function __construct()
    {
        $cl = new GuzzleHttp\Client(['base_uri' => 'https://api.badgr.io/', 'verify' => false]);

        $authorization = new Authorization();
        $auth = $authorization->auth($cl);

        if ($auth["success"])
        {
            $this->client = $cl;
            $this->authToken = $auth["token"];
            echo 'Using auth token: ' . $this->authToken . PHP_EOL;
        }
        else
        {
            echo $auth["message"] . PHP_EOL;
        }
    }

    /**
     * @param $email string
     * @param $evidenceUrl string
     * @param $badge string
     * @return mixed
     */
    function issueBadge($email, $evidenceUrl, $badge)
    {
        if($this->isDryRun()) {
            return true;
        }

        try {
            $recipient = [
                'identity' => "$email",
                'type' => 'email',
                'hashed' => true
            ];
            $evidence = [
                ['url' => "{$evidenceUrl}"]
            ];

            // Get cURL resource
            $curl = curl_init();

            curl_setopt_array($curl, array(
                CURLOPT_RETURNTRANSFER => 1,
                CURLOPT_URL => "https://api.badgr.io/v2/badgeclasses/$badge/assertions",
                CURLOPT_POST => 1,
                CURLOPT_POSTFIELDS => json_encode(array(
                    "recipient" => $recipient,
                    "evidence" => $evidence,
                    'create_notification' => true
                ), JSON_UNESCAPED_SLASHES)
            ));
            $headers = [
                "Content-Type: application/json",
                "Authorization: Bearer $this->authToken"
            ];

            curl_setopt($curl, CURLOPT_HTTPHEADER, $headers);

            // Send the request & save response to $resp
            $resp = curl_exec($curl);

            // Close request to clear up some resources
            curl_close($curl);

            return json_decode($resp, true);
        }
        catch (Exception $e)
        {
            echo "Error issuing badge: $e" . PHP_EOL;
            return false;
        }
    }
}
