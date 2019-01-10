<?php


class Badges
{
    const ISCRIZIONE_2019 = "LQZbnP-UQP26yndI7frhBQ";
    const SOSTENITORE_2019 = "RAAlvEx5Q5erX3ZzpVnc0Q";
    const SOSTENITORE_GOLD_2019 = "L9h7ijvfQfarb2zpHHGtNQ";
    const DONATORE_SPONSOR_2019 = "GEWEdYGPTlq0KXlzggxhPQ";

    const IS_ASSOCIATION = 0;
    const IS_DONATION = 1;

    static function badgeList()
    {
        return [self::ISCRIZIONE_2019 => ["type" => self::IS_ASSOCIATION, "name" => "CODEMOOC NET 2019"],
                self::SOSTENITORE_2019 => ["type" => self::IS_DONATION, "name" => "SOSTENITORE 2019"],
                self::SOSTENITORE_GOLD_2019 => ["type" => self::IS_DONATION, "name" => "SOSTENITORE GOLD 2019"],
                self::DONATORE_SPONSOR_2019  => ["type" => self::IS_DONATION, "name" => "DONATORE SPONSOR 2019"]];
    }
}