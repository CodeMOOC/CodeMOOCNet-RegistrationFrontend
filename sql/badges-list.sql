-- Show list of generated badges with donations
SELECT Badges.Email, Badges.Type, Donations.Amount FROM Badges LEFT OUTER JOIN Donations ON Badges.Email = Donations.Email ORDER BY Badges.Email;
