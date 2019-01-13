<?php
require_once 'conf.php';
require_once 'models/donator.php';

class DbConnection
{
    /**
     * @return mysqli|bool
     */
    static function Connect()
    {
        try {
            $db = new mysqli(DB_HOSTNAME, DB_USER, DB_PASSWORD, DB_NAME);

            if ($db->connect_error) {
                echo "Connection Error" . PHP_EOL;
                return false;
            }

            return $db;
        } catch (Exception $e) {
            echo "Connection Error: " . $e->getMessage() . PHP_EOL;
            return false;
        }
    }

    /**
     * @param $conn mysqli
     * @param $year string
     * @return array|bool
     */
    static function GetAllContributorsForYear($conn, $year)
    {
        try {
            $sql = "SELECT Email, Amount FROM Donations WHERE Year = '$year';";

            $result = $conn->query($sql);

            $contributors = [];
            if($result === false)
                return false;

            while ($row = $result->fetch_assoc()) {
                $contributors[] = $row;
            }

            return $contributors;
        } catch(Exception $e)
        {
            echo $sql . "<br>" . $e->getMessage();
            return false;
        }
    }

    /**
     * @param $conn mysqli
     * @param $email
     * @param $type
     * @return bool
     */
    static function GetContributorBadges($conn, $email, $type)
    {
        try {
            $sql = "SELECT * 
                    FROM Badges
                    WHERE Email = '$email' 
                    AND Type = '$type';";

            $result = $conn->query($sql);

            if($result->num_rows == 0)
                return true;

            return false;
        } catch (Exception $e)
        {
            echo $sql . "<br>" . $e->getMessage();
            return false;
        }
    }

    /**
     * @param $conn mysqli
     * @return array|bool
     */
    static function GetContributorsWithoutAssociationBadge($conn)
    {
        try {
            $sql = "SELECT Registrations.Email FROM Registrations LEFT OUTER JOIN Donations ON Registrations.Email = Donations.Email WHERE Registrations.ConfirmationTimestamp IS NOT NULL AND Donations.Amount >= 20 AND Registrations.Email NOT IN (SELECT Badges.Email FROM Badges WHERE Badges.Type = 'Iscrizione2019');";
            $result = $conn->query($sql);

            $contributors = [];
            if($result === false)
                return false;

            while ($row = $result->fetch_assoc()) {
                $contributors[] = $row['Email'];
            }

            return $contributors;
        } catch(Exception $e)
        {
            echo $sql . "<br>" . $e->getMessage();
            return false;
        }
    }

    /**
     * @param $conn mysqli
     * @param $donator Donator
     * @return bool
     */
    static function InsertDonator($conn, $donator)
    {
        try {
            $sql = "REPLACE INTO Donations (Name, Surname, Email, Year, Amount)
                    VALUES ('$donator->name', '$donator->surname', '$donator->email', '". date("Y") . "', $donator->donation);";

            $result = $conn->query($sql);
            return true;
        } catch(Exception $e)
        {
            echo $sql . "<br>" . $e->getMessage();
            return false;
        }
    }

    /**
     * @param $conn mysqli
     * @param $badgeInfo
     * @return bool
     */
    static function InsertAssignedBadge($conn, $email, $token, $type)
    {
        try {
            $date = date("Y-m-d H:i:s");
            $sql = "INSERT INTO Badges (Email, Type, IssueTimestamp, EvidenceToken)
                    VALUES ('$email', '$type', '$date', '$token')";

            $conn->query($sql);
            return true;
        } catch(Exception $e)
        {
            //echo $sql . "<br>" . $e->getMessage();
            return false;
        }
    }
}