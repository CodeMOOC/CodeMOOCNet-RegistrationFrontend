<?php
use PhpOffice\PhpSpreadsheet\Reader\Xlsx;
require 'vendor/autoload.php';

// Load spreadsheet data
LoadSpreadsheet::loadData();

class LoadSpreadsheet
{
    static function loadData()
    {
        try {
            $reader = new Xlsx();
            $spreadsheet = $reader->load('xlsx/progetto20543.xlsx');

            $sheetData = $spreadsheet->getActiveSheet()->toArray(null, true, true, true);

            foreach ($sheetData as $row)
            {   $reward = "no_badge";
                if(!empty($row['G']))
                {
                    switch ($row['G'])
                    {
                        case "Tesseramento e iscrizione":
                            $reward = "badge_iscr";
                            break;
                        case "Sostenitore":
                            $reward = "badge_sost";
                            break;
                        case "Sostenitore gold":
                            $reward = "badge_gold";
                            break;
                        case "Donatore sponsor":
                            $reward = "badge_spon";
                            break;
                    }
                }
                echo $row['E'] . ": \t\t" . $row['G']  . "\t" . $row['H'] . " €" . "\t" . $reward .  PHP_EOL;
            }
        } catch
        (\PhpOffice\PhpSpreadsheet\Exception $ex) {
            echo $ex;
        }
    }
}