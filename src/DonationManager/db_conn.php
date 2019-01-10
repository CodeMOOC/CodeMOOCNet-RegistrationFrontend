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
     */
    static function GetContributorsWithoutBadges($conn)
    {
        try {
            $sql = "SELECT Email 
                    FROM Donations 
                    LEFT JOIN Badges 
                    ON Donations.ID = Badges.DonationID
                    WHERE Badges.DonationID IS NULL;";
            $result = $conn->query($sql);

            while ($row = $result->fetch_assoc()) {
                echo $row['Email']."<br />\n";
            }
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
            $sql = "SELECT Registrations.Email 
                    FROM 
                      (SELECT * 
                      FROM Registrations 
                      WHERE ConfirmationTimestamp IS NOT NULL) Registrations 
                    LEFT JOIN Donations 
                    ON Registrations.Email = Donations.Email
                    LEFT JOIN Badges 
                    ON Registrations.Email = Badges.Email
                    WHERE Badges.Email IS NULL;";
            $result = $conn->query($sql);

            $contributors = [];
            if(!$result)
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
     * @param $donator Donator
     * @return bool
     */
    static function InsertDonator($conn, $donator)
    {
        try {
            $sql = "INSERT INTO Donations (Name, Surname, Email, Year, Amount)
                    VALUES ('$donator->name', '$donator->surname', '$donator->email', '". date("Y") . "', $donator->donation)
                    ON DUPLICATE KEY UPDATE 
                      Year = '" . date("Y") . "', 
                      Amount = $donator->donation;";

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
    static function InsertAssignedBadge($conn, $badgeInfo)
    {
        // TODO
        try {
            //$sql = "INSERT INTO Badges (name, surname, email, date, donation)
            //        VALUES ('$name', '$surname', '$email', '$date', $donation)";
            // use exec() because no results are returned
            //$conn->query($sql);
            return true;
        } catch(Exception $e)
        {
            //echo $sql . "<br>" . $e->getMessage();
            return false;
        }
    }
}