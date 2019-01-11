<?php

class SaltyHasher
{
    /**
     * @return bool|string
     */
    static function hash()
    {
        try
        {
            return hash ( "sha256" , random_bytes(512));
        } catch (Exception $e)
        {
            echo "Error generating hash data: $e" . PHP_EOL;
            return false;
        }
    }
}