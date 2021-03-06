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
        return [self::ISCRIZIONE_2019 => ["type" => self::IS_ASSOCIATION, "name" => "Iscrizione"],
                self::SOSTENITORE_2019 => ["type" => self::IS_DONATION, "name" => "Sostenitore"],
                self::SOSTENITORE_GOLD_2019 => ["type" => self::IS_DONATION, "name" => "SostenitoreGold"],
                self::DONATORE_SPONSOR_2019  => ["type" => self::IS_DONATION, "name" => "DonatoreSponsor"]];
    }
}
