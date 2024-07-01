Feature: Scrutin


Scenario: candidat2 gagne au second tour
	Given candidats
    | Candidats |
    | candidat1 |
    | candidat2 |
    | candidat3 |
	When on debute le scrutin
	And on vote 10 fois pour le candidat1
	And on vote 9 fois pour le candidat2
	And on vote 8 fois pour le candidat3
	And on cloture le premier tour
	And on vote 9 fois pour le candidat1
	And on vote 10 fois pour le candidat2
	And on cloture le second tour
	Then le gagnant doit être candidat2
	
Scenario: candidat1 gagne au premier tour
	Given candidats
	  | Candidats |
	  | candidat1 |
	  | candidat2 |
	  | candidat3 |
	When on debute le scrutin
	And on vote 20 fois pour le candidat1
	And on vote 10 fois pour le candidat2
	And on vote 5 fois pour le candidat3
	And on cloture le tour
	Then le gagnant doit être candidat1

Scenario: aucun candidat ne gagne au second tour
	Given candidats
	  | Candidats |
	  | candidat1 |
	  | candidat2 |
	  | candidat3 |
	When on debute le scrutin
	And on vote 1 fois pour le candidat1
	And on vote 1 fois pour le candidat2
	And on cloture le tour
	And on vote 1 fois pour le candidat1
	And on vote 1 fois pour le candidat2
	And on cloture le tour
	Then il n'y a pas de gagnant
	
Scenario: Impossible d'obtenir les resultats du premier tour avant sa fin
	Given candidats
	  | Candidats |
	  | candidat1 |
	  | candidat2 |
	  | candidat3 |
	When on debute le scrutin
	Then obtenir les resultats du tour 1 donne une erreur

	
Scenario: Impossible d'obtenir les resultats du second tour avant sa fin
	Given candidats
	  | Candidats |
	  | candidat1 |
	  | candidat2 |
	  | candidat3 |
	When on debute le scrutin
	And on vote 1 fois pour le candidat1
	And on vote 1 fois pour le candidat2	
	And on cloture le tour
	Then obtenir les resultats du tour 2 donne une erreur
	
Scenario: Egalite au premier tour pour candidat2 et candidat3
	Given candidats
	  | Candidats |
	  | candidat1 |
	  | candidat2 |
	  | candidat3 |
	When on debute le scrutin
	And on vote 3 fois pour le candidat1
	And on vote 2 fois pour le candidat2	
	And on vote 2 fois pour le candidat3	
	And on cloture le tour
	Then seulement le candidat2 ou le candidat3 passe au second tour