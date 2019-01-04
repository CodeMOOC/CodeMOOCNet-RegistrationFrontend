<?php

/* signup form data */
$name = $_POST["name"];
$surname = $_POST["surname"];
$email = $_POST["email"];
$birthdate = $_POST["birthdate"];
$birthLocation = $_POST["birthLocation"];
$password = $_POST["password"];
$confirmPassword = $_POST["confirmPassword"];

$phone = "";
if(!empty($_POST["phone"]))
    $phone = $_POST["phone"];
$teacher = false;
if(!empty($_POST["teacher"]))
    $teacher = true;
$mooc = false;
if(!empty($_POST["mooc"]))
    $mooc = true;
$privacy = false;
if(!empty($_POST["privacy"]))
    $privacy = true;
$gdpr = false;
if(!empty($_POST["gdpr"]))
    $gdpr = true;


// DATABASE INFO:
// Tutti i campi utente, aggiungendo:
// - data di registrazione
// - id utente univoco (utilizzo del token univoco?)
// - token univoco per l'attivazione dell'account
// - stato utente: richiesta di registrazione inviata | richiesta di registrazione confermata in seguito a pagamento
// - stato tesseramento annuale: pagato | non pagato
// - data pagamento
// - valore quota pagata
// Bisognerà tenere conto in futuro anche dello storico dei pagamenti