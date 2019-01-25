<?php
require 'vendor/autoload.php';
require_once 'models/donator.php';

use PhpOffice\PhpSpreadsheet\Reader\Xlsx;

class LoadSpreadsheet
{
    /**
     * @return array|bool
     */
    static function loadData()
    {
        try {
            $reader = new Xlsx();
            $spreadsheet = $reader->load('donations.xlsx');

            $sheetData = $spreadsheet->getActiveSheet()->toArray(null, true, true, true);
            $donators = [];

            foreach ($sheetData as $row)
            {
                if($row['A'] == 'Data' || $row['A'] == 'Date' || empty($row['A']))
                    continue;

                $donator = new Donator($row);
                if(isset($donators[$donator->email])) {
                    echo "Donor $donator->email donated multiple times (" . $donators[$donator->email]->donation . " + $donator->donation)" . PHP_EOL;
                    // aggregate data if user donated more than once
                    $donator->donation = $donators[$donator->email]->donation + $donator->donation;
                    $donator->date = $donators[$donator->email]->date;
                }
                $donators[$donator->email] = $donator;
            }
            return $donators;
        } catch
        (\PhpOffice\PhpSpreadsheet\Exception $ex) {
            echo $ex;
            return false;
        }
    }
}