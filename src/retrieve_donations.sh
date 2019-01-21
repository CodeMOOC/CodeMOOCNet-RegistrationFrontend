#! /bin/bash

echo "Performing fake login to Produzioni dal Basso Website..."
curl -s --cookie-jar pdb_cookies https://www.produzionidalbasso.com/ > /dev/null
PDB_TOKEN=$(awk 'NR>4 { print $7 }' pdb_cookies)
echo "CSRF token: $PDB_TOKEN"
curl -s -X POST -F "login-username=$PDB_LOGIN" -F "login-password=$PDB_PASSWORD" -F "csrfmiddlewaretoken=$PDB_TOKEN" --cookie pdb_cookies --cookie-jar pdb_cookies --referer https://www.produzionidalbasso.com/ https://www.produzionidalbasso.com/account/login_check
curl -s --cookie pdb_cookies --referer https://www.produzionidalbasso.com/account/project/${PDB_PROJECT_ID} -o donations.xlsx https://www.produzionidalbasso.com/account/project/backers/${PDB_PROJECT_ID}/export

# Clean up cookie jar
rm pdb_cookies
