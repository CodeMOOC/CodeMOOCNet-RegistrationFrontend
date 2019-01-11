<?php
require 'vendor/autoload.php';

/**
 * Class Authorization
 */
class Authorization
{
    /**
     * @param $client GuzzleHttp\Client
     * @return array
     */
    function auth($client)
    {
        try {
            $uri = 'o/token';
            $response = null;
            $response = $client->request("POST", $uri,
                array(
                    'form_params' => [
                        'username' => getenv('BADGR_ISSUER_EMAIL'),
                        'password' => getenv('BADGR_ISSUER_PASSWORD')
                    ]
                )
            );
            $body = $response->getBody();

            // Implicitly cast the body to a string to get token
            $json = json_decode($body, true);

            return ["success" => true,
                "token" => $json['access_token']];

        } catch (GuzzleHttp\Exception\GuzzleException $guzzEx) {
            return ["success" => false,
                "message" => $guzzEx];
        } catch (Exception $e) {
            return ["success" => false,
                "message" => $e];
        }
    }
}