<?php

class Donator
{
    public $name;
    public $surname;
    public $email;
    public $date;
    public $donation;

    public function __construct($cellRow)
    {
        $this->name = $cellRow['C'];
        $this->surname = $cellRow['D'];
        $this->email = $cellRow['E'];
        $this->date = $cellRow['A'];
        $this->donation = $cellRow['H'];
    }
}