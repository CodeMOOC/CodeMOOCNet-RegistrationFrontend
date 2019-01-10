<?php
require_once '../config.php';
require '../models/donator.php';

class DbConnection
{
    /**
     * @return PDO|string
     */
    static function Connect()
    {
        try {
            $db = new PDO ("mysql:host=" . DB_HOSTNAME . ";dbname=" . DB_NAME, DB_USER, DB_PASSWORD);
            return $db;
        } catch (PDOException $e) {
            return "Connection Error: " . $e->getMessage();
        }
    }

    /**
     * @param $conn PDO
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

            while ($row = $result->fetch()) {
                echo $row['Email']."<br />\n";
            }
        } catch (PDOException $e)
        {
            echo $sql . "<br>" . $e->getMessage();
            return false;
        }
    }

    /**
     * @param $conn PDO
     */
    static function GetContributorsWithoutAssociationBadge($conn)
    {
        try {
            $sql = "SELECT Email 
                    FROM 
                      (SELECT * 
                      FROM Registrations 
                      WHERE ConfirmationTimestamp IS NOT NULL) Registrations 
                    LEFT JOIN Donations 
                    ON Registrations.Email = Donations.Email
                    LEFT JOIN Badges 
                    ON Registrations.Email = Badges.Email;";
            $result = $conn->query($sql);

            $contributors = [];
            while ($row = $result->fetch()) {
                var_dump($row['Email']);
                $contributors[] = $row;
            }

            return $contributors;
        } catch(PDOException $e)
        {
            echo $sql . "<br>" . $e->getMessage();
            return false;
        }
    }

    /**
     * @param $conn PDO
     * @param $donator Donator
     * @return bool
     */
    static function InsertDonator($conn, $donator)
    {
        try {
            $sql = "INSERT INTO Donations (Name, Surname, Email, Date, Donation)
                    VALUES ('$donator->name', '$donator->surname', '$donator->email', '$donator->date', $donator->donation)
                    ON DUPLICATE KEY UPDATE Date = '$donator->date' AND Donation = $donator->donation;";
            // use exec() because no results are returned
            $conn->exec($sql);
            return true;
        } catch(PDOException $e)
        {
            echo $sql . "<br>" . $e->getMessage();
            return false;
        }
    }

    /**
     * @param $conn
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
            //$conn->exec($sql);
            return true;
        } catch(PDOException $e)
        {
            //echo $sql . "<br>" . $e->getMessage();
            return false;
        }
    }
}