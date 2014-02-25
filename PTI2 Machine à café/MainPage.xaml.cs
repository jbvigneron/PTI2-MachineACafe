using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PTI2_Machine_à_café
{
	public partial class MainPage : UserControl
	{	
        double NbSucreAVerser; // Nombre de sucre à verser (par défaut 3)
        int NbSucresVerses; // Nombre de sucres tombés dans la tasse
		int monnaie; // Monnaie insérée dans la machine
		int numeroTexte; // Pour le timer. Id du texte à afficher
        DispatcherTimer timerDemarrage; // Timer enclenché au démarrage pour afficher des textes à la suite
        DispatcherTimer timerSucre; // Timer retardant la descente du sucre dans le gobelet
        DispatcherTimer timerRetour; // Timer qui s'enclenche après avoir changé la quantité de sucre

        // Chargement au départ
		public MainPage()
		{
			// Requis pour initialiser des variables
			InitializeComponent();

            timerDemarrage = new DispatcherTimer();
            timerSucre = new DispatcherTimer();
            timerRetour = new DispatcherTimer();

            // Note: abonnement dans le code-behind car bug si l'abonnement se fait depuis le code XAML
            scrollSucre.ValueChanged +=new RoutedPropertyChangedEventHandler<double>(scrollSucre_ValueChanged);
        }

        // Chargement de la page
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            NbSucreAVerser = 3;
            NbSucresVerses = 0;
            monnaie = 0;
            numeroTexte = 0;
            
            eteindreMachine(); // Eteindre la machine au chargement de la page
			
			glisserGobelet.Completed +=new System.EventHandler(glisserGobelet_Completed);
			versement.Completed +=new System.EventHandler(versement_Completed);
			versementSucre.Completed +=new System.EventHandler(versementSucre_Completed);
        }

        // Procédure qui active la machine
        private void allumerMachine()
        {
            // Désactiver le Bouton On pour éviter qu'on puisse recliquer dessus et activer le bouton Off
            boutonOn.IsEnabled = false;
            boutonOff.IsEnabled = true;

            numeroTexte = 0; // Compteur pour le timer pour afficher les textes (selon la valeur de ce compteur)
            afficheurHaut.Text = "Initialisation";

            // Enclencher le timer
            timerDemarrage.Interval = new TimeSpan(0, 0, 1); // 1 seconde
            timerDemarrage.Tick += new EventHandler(timerDemarrage_Tick);
            timerDemarrage.Start();
        }

        // Procédure qui éteint la machine
        private void eteindreMachine()
        {
            // Désactiver le Bouton Off pour éviter qu'on puisse recliquer dessus et activer le bouton On
            boutonOff.IsEnabled = false;
            boutonOn.IsEnabled = true;

            activerElements(false); // Désactiver les boutons de sélection
            afficherPieces(Visibility.Visible, Visibility.Collapsed); // Afficher le bouton "Insérer la monnaie", "Rendre la monnaie" et masquer les pièces
            boutonServir.Visibility = Visibility.Collapsed; // Masquer le bouton "Se Servir"
            gobelet2.Visibility = Visibility.Collapsed; // Masquer le gobelet (si le checkbox Gobelet est coché)
            listeRenduMonnaie.Items.Clear(); // Vider la liste du rendu monnaie

            // Remettre à 0 les animations
            glisserGobelet.Stop();
            versement.Stop();
            versementSucre.Stop();

            // Stopper les timers
            timerDemarrage.Stop();
            timerRetour.Stop();
            timerSucre.Stop();

            // Afficher un message pour l'utilisateur
            afficheurHaut.Text = "Hors service";
            afficheurBas.Text = "";
        }

        // Procédure qui active ou désactiver les boutons de sélection
        private void activerElements(bool etat)
        {
            boutonInsererMonnaie.IsEnabled = etat;
            scrollSucre.IsEnabled = etat;
            cbGobelet.IsEnabled = etat;

            boutonCafe.IsEnabled = etat;
            boutonChocolat.IsEnabled = etat;
            boutonCapuccino.IsEnabled = etat;
            boutonLait.IsEnabled = etat;
            boutonTomates.IsEnabled = etat;
            boutonEau.IsEnabled = etat;
        }

        // Procédure qui masque (affiche) le bouton "Insérer la monnaie" et affiche (masque) les pièces
        private void afficherPieces(Visibility etat1, Visibility etat2)
        {
            boutonInsererMonnaie.Visibility = etat1;
            boutonRendreMonnaie.Visibility = etat2;

            piece5centimes.Visibility = etat2;
            piece10centimes.Visibility = etat2;
            piece20centimes.Visibility = etat2;
            piece50centimes.Visibility = etat2;
            piece1euro.Visibility = etat2;
            piece2euros.Visibility = etat2;
        }

        // Procédure qui ajoute la monnaie
        private void ajoutMonnaie(int rajout)
        {
            // Si il y a moins de 2€ inséré, l'ajout de pièces est possible
            if (monnaie < 200)
            {
                // Stopper le timer si l'utilisateur a bougé le scroll du sucre ou coché la case pour le gobelet
                timerRetour.Stop();

                // Incrémenter la monnaie et l'afficher
                monnaie = monnaie + rajout;

                /* Afficher le rendu monnaie sur l'afficheur
                Si la monnaie rendue est égale à 5 centimes, on ajoute un 0 */
                if (monnaie % 100 == 5)
                {
                    afficheurBas.Text = "Monnaie: " + monnaie / 100 + ",0" + monnaie % 100 + "€";
                }
                else
                {
                    afficheurBas.Text = "Monnaie: " + monnaie / 100 + "," + monnaie % 100 + "€";
                }
            }
        }

        // Procédure qui verse le liquide dans le gobelet
        private void verser()
        {
            timerRetour.Stop();

            // Vider la liste de rendu monnaie
            listeRenduMonnaie.Items.Clear();

            // Vérifier s'il y a assez de monnaie
            if (monnaie >= 40)
            {
                // Désactiver les boutons de sélection
                activerElements(false);

                // Affiche le bouton "Insérer la monnaie" et masque les pièces
                afficherPieces(Visibility.Visible, Visibility.Collapsed);

                // Désactiver le bouton "Insérer monnaie"
                boutonInsererMonnaie.IsEnabled = false;

                // Si l'utilisateur a coché "Gobelet", on active l'animation
                if (cbGobelet.IsChecked == true)
                {
                    // Afficher un message pour l'utilisateur
                    afficheurHaut.Text = "Traitement en cours";
                    afficheurBas.Text = "Veuillez patienter";

                    // Déclencher l'animation 
                    glisserGobelet.Begin();

                    // Si l'utilisateur a demandé du sucre, déclencher l'animation qui fait tomber le sucre
                    if (scrollSucre.Value >= 1)
                    {
                        NbSucresVerses = 0;

                        // Enclencher la descente du sucre avec un délai de 2 secondes
                        timerSucre.Interval = new TimeSpan(0, 0, 2); // 2 seconde
                        timerSucre.Tick += new System.EventHandler(timerSucre_Tick);
                        timerSucre.Start();
                    }
                }
                // Sinon on affiche à l'utilisateur un bouton lui permettant de glisser son gobelet
                else
                {
                    boutonInsererGobelet.Visibility = Visibility.Visible;
                }
            }
            // Si il n'y a pas assez de monnaie, afficher le prix d'une boisson
            else
            {
                afficheurBas.Text = "Prix: 0.40€";
            }
        }

        // Procédure qui rend la monnaie
        private void renduMonnaie()
        {
            // Initialisation des variables qui comptent le nombre de pièces
            int nbPieces2euros = 0;
            int nbPieces1euro = 0;
            int nbPieces50centimes = 0;
            int nbPieces20centimes = 0;
            int nbPieces10centimes = 0;
            int nbPieces5centimes = 0;

            /* Afficher le rendu monnaie sur l'afficheur
            Si la monnaie rendue est égale à 5 centimes, on ajoute un 0 */
            if (monnaie % 100 == 5)
            {
                afficheurBas.Text = "Rendu monnaie: " + monnaie / 100 + ",0" + monnaie % 100 + "€";
            }
            else
            {
                afficheurBas.Text = "Rendu monnaie: " + monnaie / 100 + "," + monnaie % 100 + "€";
            }

            // Rendre toute la monnaie
            while (monnaie >= 200)
            {
                monnaie = monnaie - 200;
                nbPieces2euros++;
            }

            while (monnaie >= 100)
            {
                monnaie = monnaie - 100;
                nbPieces1euro++;
            }

            while (monnaie >= 50)
            {
                monnaie = monnaie - 50;
                nbPieces50centimes++;
            }

            while (monnaie >= 20)
            {
                monnaie = monnaie - 20;
                nbPieces20centimes++;
            }

            while (monnaie >= 10)
            {
                monnaie = monnaie - 10;
                nbPieces10centimes++;
            }

            while (monnaie >= 5)
            {
                monnaie = monnaie - 5;
                nbPieces5centimes++;
            }

            // Afficher le nombre de pièces rendues dans la Listbox
            if (nbPieces2euros > 0)
            {
                listeRenduMonnaie.Items.Add(nbPieces2euros + " pièces de 2€");
            }

            if (nbPieces1euro > 0)
            {
                listeRenduMonnaie.Items.Add(nbPieces1euro + " pièces de 1€");
            }

            if (nbPieces50centimes > 0)
            {
                listeRenduMonnaie.Items.Add(nbPieces50centimes + " pièces de 50 centimes");
            }

            if (nbPieces20centimes > 0)
            {
                listeRenduMonnaie.Items.Add(nbPieces20centimes + " pièces de 20 centimes");
            }

            if (nbPieces10centimes > 0)
            {
                listeRenduMonnaie.Items.Add(nbPieces10centimes + " pièces de 10 centimes");
            }

            if (nbPieces5centimes > 0)
            {
                listeRenduMonnaie.Items.Add(nbPieces5centimes + " pièces de 5 centimes");
            }

            monnaie = 0;
            afficherPieces(Visibility.Visible, Visibility.Collapsed); // Masquer les pièces et afficher les boutons

            // Enclencher le timer pour réafficher "En service | Faîtes votre choix"
            timerRetour.Interval = new TimeSpan(0, 0, 3); // 3 secondes
            timerRetour.Tick += new System.EventHandler(timerRetour_Tick);
            timerRetour.Start();
        }
		
		// Bouton "On"
		private void boutonOn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            allumerMachine();
		}

        // Bouton "Off"
        private void boutonOff_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            eteindreMachine();
        }
		
		// Timer pour afficher les textes à la suite
		private void timerDemarrage_Tick(object o, EventArgs sender)
		{
            if (numeroTexte == 0)
			{
				afficheurBas.Text = "Mise en route";
				timerDemarrage.Interval = new TimeSpan(0, 0, 1);
			}
            else if (numeroTexte == 1)
			{
				afficheurBas.Text = "En chauffe";
				timerDemarrage.Interval = new TimeSpan(0, 0, 3);
			}
            else if (numeroTexte == 2)
			{
				afficheurHaut.Text = "En service";
				afficheurBas.Text = "Faites votre choix";	
				timerDemarrage.Stop();
                activerElements(true); // Activer les boutons de sélection
			}
			
            numeroTexte++;
		}

        // Timer permettant de réafficher "En service | Faîtes votre choix"
        private void timerRetour_Tick(object sender, System.EventArgs e)
        {
            // Stopper le timer
            timerRetour.Stop();

            // Afficher le texte
            afficheurHaut.Text = "En service";
            afficheurBas.Text = "Faites votre choix";
        }

        // Démarrer le versement du sucre avec un certains délai
        private void timerSucre_Tick(object sender, System.EventArgs e)
        {
            timerSucre.Stop();
            versementSucre.Begin();
        }
		
		// Image "5 centimes"
		private void piece5centimes_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            ajoutMonnaie(5); // Ajouter 5 centimes à la monnaie insérée
		}

		// Image "10 centimes"
		private void piece10centimes_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            ajoutMonnaie(10); // Ajouter 10 centimes à la monnaie insérée
		}

		// Image "20 centimes"
		private void piece20centimes_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            ajoutMonnaie(20); // Ajouter 20 centimes à la monnaie insérée
		}

		// Image "50 centimes"
		private void piece50centimes_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            ajoutMonnaie(50); // Ajouter 50 centimes à la monnaie insérée
		}

		// Image "1 euro"
		private void piece1euro_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            ajoutMonnaie(100); // Ajouter 1€ à la monnaie insérée
		}

		// Image "2 euros"
		private void piece2euros_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            ajoutMonnaie(200); // Ajouter 2€ à la monnaie insérée
		}
		
		// Bouton de sélection "Café"
		private void boutonCafe_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// Définir la couleur du liquide
			SolidColorBrush couleurCafe = new SolidColorBrush();
			couleurCafe.Color = Color.FromArgb(255, 34, 6, 6);
			
			boissonDescend.Fill = couleurCafe;
			boissonLever.Fill = couleurCafe;

            verser(); // Verser le liquide dans le gobelet
		}

		// Bouton de sélection "Chocolat"
		private void boutonChocolat_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// Définir la couleur du liquide
			SolidColorBrush couleurChocolat = new SolidColorBrush();
			couleurChocolat.Color = Color.FromArgb(255, 81, 45, 16);
			
			boissonDescend.Fill = couleurChocolat;
			boissonLever.Fill = couleurChocolat;

            verser(); // Verser le liquide dans le gobelet
		}
		
		// Bouton de sélection "Capuccino"
		private void boutonCapuccino_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// Définir la couleur du liquide
			SolidColorBrush couleurCapuccino = new SolidColorBrush();
			couleurCapuccino.Color = Color.FromArgb(255, 193, 124, 4);
			
			boissonDescend.Fill = couleurCapuccino;
			boissonLever.Fill = couleurCapuccino;

            verser(); // Verser le liquide dans le gobelet
		}

		// Bouton de sélection "Lait"
		private void boutonLait_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// Définir la couleur du liquide
			SolidColorBrush couleurLait = new SolidColorBrush();
			couleurLait.Color = Color.FromArgb(255, 255, 255, 255);
			
			boissonDescend.Fill = couleurLait;
			boissonLever.Fill = couleurLait;

            verser(); // Verser le liquide dans le gobelet
		}
		
		// Bouton de sélection "Potage Tomates"
		private void boutonTomates_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// Définir la couleur du liquide
			SolidColorBrush couleurTomates = new SolidColorBrush();
			couleurTomates.Color = Color.FromArgb(255, 255, 0, 0);
			
			boissonDescend.Fill = couleurTomates;
			boissonLever.Fill = couleurTomates;

            verser(); // Verser le liquide dans le gobelet
		}	
		
		// Bouton de sélection "Eau"
		private void boutonEau_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// Définir la couleur du liquide
			SolidColorBrush couleurEau = new SolidColorBrush();
			couleurEau.Color = Color.FromArgb(255, 168, 208, 204);
			
			boissonDescend.Fill = couleurEau;
			boissonLever.Fill = couleurEau;

            verser(); // Verser le liquide dans le gobelet
		}

        // Bouton "Rendre la monnaie"
        private void boutonRendreMonnaie_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (monnaie > 0)
            {
                listeRenduMonnaie.Items.Clear();
                renduMonnaie();
            }
        }

        // Bouton "Se servir"
        private void boutonServir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            versement.Stop();
            glisserGobelet.Stop();

            listeRenduMonnaie.Items.Clear();

            boutonServir.Visibility = Visibility.Collapsed;
            gobelet2.Visibility = Visibility.Collapsed;

            boutonInsererMonnaie.IsEnabled = true;
            activerElements(true);
        }

        // Bouton "Insérer la Monnaie"
        private void boutonInsererMonnaie_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            afficherPieces(Visibility.Collapsed, Visibility.Visible); // Masquer le bouton "Insérer la monnaie" et afficher les pièces
        }

        // Bouton "Insérer Gobelet"
        private void boutonInsererGobelet_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Afficher un message pour l'utilisateur
            afficheurHaut.Text = "Traitement en cours";
            afficheurBas.Text = "Veuillez patienter";

            // Masquer le bouton "Insérer le gobelet
            boutonInsererGobelet.Visibility = Visibility.Collapsed;

            // Afficher le gobelet et déclencher l'animation 
            gobelet2.Visibility = Visibility.Visible;
            versement.Begin();

            // Si l'utilisateur a demandé du sucre, déclencher l'animation qui fait tomber le sucre
            if (scrollSucre.Value >= 1)
            {
                // Pour compter le nombre de sucres à faire tomber
                NbSucresVerses = 0;

                // Enclencher la descente du sucre avec un délai de 2 secondes
                timerSucre.Interval = new TimeSpan(0, 0, 2); // 2 seconde
                timerSucre.Tick += new System.EventHandler(timerSucre_Tick);
                timerSucre.Start();
            }
        }
		
        // Fin de l'animation "Arrivée du gobelet"
        private void glisserGobelet_Completed(object sender, System.EventArgs e)
        {
            versement.Begin();
        }
		
        // Fin de la tombée du sucre
		private void versementSucre_Completed(object sender, System.EventArgs e)
		{
            versementSucre.Stop(); // Remettre le sucre en place
            NbSucresVerses++;

			// Refaire tomber un sucre si necessaire
			if(NbSucresVerses < NbSucreAVerser)
			{
				versementSucre.Begin();
			}	
		}	
		
        // Fin du versement de la boisson
		private void versement_Completed(object sender, System.EventArgs e)
		{
			// Afficher un message et le bouton "Se servir"
			afficheurHaut.Text = "Servez-vous";
			boutonServir.Visibility = Visibility.Visible;

            // On enlève 40 centimes pour faire les comptes
            monnaie = monnaie - 40;

            // Rendre la monnaie
			renduMonnaie();
		}

        // Scroller "Nombre de sucres"
        private void scrollSucre_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            NbSucreAVerser = scrollSucre.Value; // Stocker la valeur sélectionnée dans la variable "sucre"
            afficheurBas.Text = "Sucre: ";  // Afficher "Sucre:" et un nombre de "0" en fonction de la valeur sélectionné

            for (int i = 1; i <= scrollSucre.Value; i++)
            {
                afficheurBas.Text += "0 ";
            }

            // Enclencher le timer pour réafficher "En service | Faîtes votre choix"
            timerRetour.Interval = new TimeSpan(0, 0, 3); // 3 secondes
            timerRetour.Tick += new System.EventHandler(timerRetour_Tick);
            timerRetour.Start();
        }
		
        // Checkbox "Gobelet"
		private void cbGobelet_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if(cbGobelet.IsChecked == true)
			{
				afficheurBas.Text = "Gobelet: Oui";
			}
			else
			{
				afficheurBas.Text = "Gobelet: Non";
			}
			
			// Enclencher le timer pour réafficher "En service | Faîtes votre choix"
    		timerRetour.Interval = new TimeSpan(0, 0, 3); // 3 secondes
    		timerRetour.Tick +=new System.EventHandler(timerRetour_Tick);
			timerRetour.Start();
		}

	}
}