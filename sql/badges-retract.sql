-- Show list of badges to rectract (either no payment or payment too low)
SELECT Badges.Email, Badges.Type, Donations.Amount FROM Badges LEFT OUTER JOIN Donations ON Badges.Email = Donations.Email WHERE Donations.Amount IS NULL OR Donations.Amount < CASE Badges.Type
  WHEN 'Iscrizione2019' THEN 20
  WHEN 'Sostenitore2019' THEN 50
  WHEN 'SostenitoreGold2019' THEN 100
  WHEN 'DonatoreSponsor2019' THEN 1000
  ELSE 0
END;
