using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PTI2_Machine_à_café
{
    /// <summary>
    /// Page principale
    /// </summary>
    public partial class MainPage
    {
        #region Attributs

        /// <summary>
        /// Nombre de sucre à verser (par défaut 3)
        /// </summary>
        private double _nbSucreAVerser = 3;

        /// <summary>
        /// Nombre de sucres tombés dans la tasse
        /// </summary>
        private int _nbSucresVerses;

        /// <summary>
        /// Monnaie insérée dans la machine
        /// </summary>
        private double _monnaie;

        /// <summary>
        /// // Pour le timer. Id du texte à afficher
        /// </summary>
        private int _numeroTexte;

        /// <summary>
        /// // Timer enclenché au démarrage pour afficher des textes à la suite
        /// </summary>
        private readonly DispatcherTimer _timerDemarrage = new DispatcherTimer();

        /// <summary>
        /// Timer retardant la descente du sucre dans le gobelet
        /// </summary>
        private readonly DispatcherTimer _timerSucre = new DispatcherTimer();

        /// <summary>
        /// Timer qui s'enclenche après avoir changé la quantité de sucre
        /// </summary>
        private readonly DispatcherTimer _timerRetour = new DispatcherTimer();

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            scrollSucre.ValueChanged += scrollSucre_ValueChanged;
        }

        #endregion

        #region Evènements de la page

        /// <summary>
        /// Chargement de la page
        /// </summary>
        /// <param name="sender">Page</param>
        /// <param name="e">Event arguments</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Eteindre la machine au chargement de la page
            EteindreMachine();

            // Abonnement aux évènements
            glisserGobelet.Completed += glisserGobelet_Completed;
            versement.Completed += versement_Completed;
            versementSucre.Completed += versementSucre_Completed;

            _timerDemarrage.Interval = new TimeSpan(0, 0, 1); // 1 seconde
            _timerDemarrage.Tick += timerDemarrage_Tick;

            _timerSucre.Interval = new TimeSpan(0, 0, 2); // 2 seconde
            _timerSucre.Tick += timerSucre_Tick;

            _timerRetour.Interval = new TimeSpan(0, 0, 3); // 3 secondes
            _timerRetour.Tick += timerRetour_Tick;
        }

        #endregion

        #region Evènements des boutons d'activation/désactivation de la machine

        /// <summary>
        /// Click sur le bouton "On"
        /// </summary>
        /// <param name="sender">Bouton "On"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonOn_Click(object sender, RoutedEventArgs e)
        {
            AllumerMachine();
        }

        /// <summary>
        /// Click sur le bouton "Off"
        /// </summary>
        /// <param name="sender">Bouton "Off"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonOff_Click(object sender, RoutedEventArgs e)
        {
            EteindreMachine();
        }

        #endregion

        #region Evènements des timers

        /// <summary>
        /// Timer pour afficher les textes à la suite
        /// </summary>
        /// <param name="sender">Timer pour le sucre</param>
        /// <param name="e">Arguments de l'évèement</param>
        private void timerDemarrage_Tick(object sender, EventArgs e)
        {
            if (_numeroTexte == 0)
            {
                afficheurBas.Text = "Mise en route";
                _timerDemarrage.Interval = new TimeSpan(0, 0, 1);
            }
            else if (_numeroTexte == 1)
            {
                afficheurBas.Text = "En chauffe";
                _timerDemarrage.Interval = new TimeSpan(0, 0, 3);
            }
            else if (_numeroTexte == 2)
            {
                afficheurHaut.Text = "En service";
                afficheurBas.Text = "Faites votre choix";
                _timerDemarrage.Stop();

                // Activer les boutons de sélection
                ActiverElements(true);
            }

            _numeroTexte++;
        }

        /// <summary>
        /// Timer permettant de réafficher "En service | Faîtes votre choix"
        /// </summary>
        /// <param name="sender">Timer retour</param>
        /// <param name="e">Arguments de l'évèement</param>
        private void timerRetour_Tick(object sender, EventArgs e)
        {
            // Stopper le timer
            _timerRetour.Stop();

            // Afficher le texte
            afficheurHaut.Text = "En service";
            afficheurBas.Text = "Faites votre choix";
        }

        /// <summary>
        /// Démarrer le versement du sucre avec un certains délai
        /// </summary>
        /// <param name="sender">Timer pour le sucre</param>
        /// <param name="e">Arguments de l'évèement</param>
        private void timerSucre_Tick(object sender, EventArgs e)
        {
            _timerSucre.Stop();
            versementSucre.Begin();
        }

        #endregion

        #region Evènements des pièces

        /// <summary>
        /// Click sur l'image "5 centimes"
        /// </summary>
        /// <param name="sender">Image "5 centimes"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void piece5centimes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AjoutMonnaie(0.05);
        }

        /// <summary>
        /// Click sur l'image "10 centimes"
        /// </summary>
        /// <param name="sender">Image "10 centimes"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void piece10centimes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AjoutMonnaie(0.10);
        }

        /// <summary>
        /// Click sur l'image "20 centimes"
        /// </summary>
        /// <param name="sender">Image "20 centimes"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void piece20centimes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AjoutMonnaie(0.20); // Ajouter 20 centimes à la monnaie insérée
        }

        /// <summary>
        /// Click sur l'image "50 centimes"
        /// </summary>
        /// <param name="sender">Image "50 centimes"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void piece50centimes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AjoutMonnaie(0.50);
        }

        /// <summary>
        /// Click sur l'image "1 euro"
        /// </summary>
        /// <param name="sender">Image "1 euro"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void piece1euro_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AjoutMonnaie(1.0);
        }

        /// <summary>
        /// Click sur l'image "2 euros"
        /// </summary>
        /// <param name="sender">Image "2 euros"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void piece2euros_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AjoutMonnaie(2.0);
        }

        #endregion

        #region Evènements des boutons de sélection

        /// <summary>
        /// Click sur le bouton de sélection "Café"
        /// </summary>
        /// <param name="sender">Bouton de sélection "Café"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonCafe_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromArgb(255, 34, 6, 6);
            Verser(color);
        }

        /// <summary>
        /// Click sur le bouton de sélection "Chocolat"
        /// </summary>
        /// <param name="sender">Bouton de sélection "Chocolat"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonChocolat_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromArgb(255, 81, 45, 16);
            Verser(color);
        }

        /// <summary>
        /// Click sur le bouton de sélection "Capuccino"
        /// </summary>
        /// <param name="sender">Bouton de sélection "Capuccino"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonCapuccino_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromArgb(255, 193, 124, 4);
            Verser(color);
        }

        /// <summary>
        /// Click sur le bouton de sélection "Lait"
        /// </summary>
        /// <param name="sender">Bouton de sélection "Lait"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonLait_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromArgb(255, 255, 255, 255);
            Verser(color);
        }

        /// <summary>
        /// Click sur le bouton de sélection "Potage Tomates"
        /// </summary>
        /// <param name="sender">Bouton de sélection "Potage Tomates"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonTomates_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromArgb(255, 255, 0, 0);
            Verser(color);
        }

        /// <summary>
        /// Click sur le bouton de sélection "Eau"
        /// </summary>
        /// <param name="sender">Bouton de sélection "Eau"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonEau_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromArgb(255, 168, 208, 204);
            Verser(color);
        }

        #endregion

        #region Evènements des animations

        /// <summary>
        /// Fin de l'animation "Arrivée du gobelet"
        /// </summary>
        private void glisserGobelet_Completed(object sender, EventArgs e)
        {
            versement.Begin();
        }

        /// <summary>
        /// Fin de la tombée du sucre
        /// </summary>
        private void versementSucre_Completed(object sender, EventArgs e)
        {
            versementSucre.Stop(); // Remettre le sucre en place
            _nbSucresVerses++;

            // Refaire tomber un sucre si necessaire
            if (_nbSucresVerses < _nbSucreAVerser)
            {
                versementSucre.Begin();
            }
        }

        /// <summary>
        /// Fin du versement de la boisson
        /// </summary>
        private void versement_Completed(object sender, EventArgs e)
        {
            // Afficher un message et le bouton "Se servir"
            afficheurHaut.Text = "Servez-vous";
            boutonServir.Visibility = Visibility.Visible;

            // On enlève 40 centimes pour faire les comptes
            _monnaie = _monnaie - 0.4;

            // Rendre la monnaie
            RenduMonnaie();
        }

        #endregion

        #region Autres évènements

        /// <summary>
        /// Click sur le bouton "Rendre la monnaie"
        /// </summary>
        /// <param name="sender">Bouton "Rendre la monnaie"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonRendreMonnaie_Click(object sender, RoutedEventArgs e)
        {
            if (_monnaie > 0)
            {
                listeRenduMonnaie.Items.Clear();
                RenduMonnaie();
            }
        }

        /// <summary>
        /// Click sur le bouton "Se servir"
        /// </summary>
        /// <param name="sender">Bouton "Se servir"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonServir_Click(object sender, RoutedEventArgs e)
        {
            versement.Stop();
            glisserGobelet.Stop();

            listeRenduMonnaie.Items.Clear();

            boutonServir.Visibility = Visibility.Collapsed;
            gobelet2.Visibility = Visibility.Collapsed;

            boutonInsererMonnaie.IsEnabled = true;
            ActiverElements(true);
        }

        /// <summary>
        /// Masquer le bouton "Insérer la monnaie" et afficher les pièces
        /// </summary>
        /// <param name="sender">Bouton "Insérer la Monnaie"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void boutonInsererMonnaie_Click(object sender, RoutedEventArgs e)
        {
            AfficherPieces(Visibility.Collapsed, Visibility.Visible);
        }

        /// <summary>
        /// Déclenché lorsque l'utilisateur insère son gobelet
        /// </summary>
        /// <param name="sender">Bouton "Insérer Gobelet"</param>
        /// <param name="e"></param>
        private void boutonInsererGobelet_Click(object sender, RoutedEventArgs e)
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
                _nbSucresVerses = 0;

                _timerSucre.Start();
            }
        }

        /// <summary>
        /// Déclencghé lorsque l'utilisateur change la valeur du scroller "Nombre de sucres"
        /// </summary>
        /// <param name="sender">Scroller "Nombre de sucres"</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void scrollSucre_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Stocker la valeur sélectionnée dans la variable "sucre"
            _nbSucreAVerser = scrollSucre.Value;

            // Afficher "Sucre:" et un nombre de "0" en fonction de la valeur sélectionné
            afficheurBas.Text = "Sucre: ";

            for (int i = 1; i <= scrollSucre.Value; i++)
            {
                afficheurBas.Text += "0 ";
            }

            _timerRetour.Start();
        }

        /// <summary>
        /// Déclenché lorsque l'utilisateur coche/décoche le checkbox "Gobelet ?" 
        /// </summary>
        /// <param name="sender">Checkbox "Gobelet ?"</param>
        /// <param name="e"></param>
        private void cbGobelet_Click(object sender, RoutedEventArgs e)
        {
            afficheurBas.Text = cbGobelet.IsChecked.HasValue && cbGobelet.IsChecked.Value ? "Gobelet: Oui" : "Gobelet: Non";
            _timerRetour.Start();
        }

        #endregion

        #region Méthodes

        /// <summary>
        /// Procédure qui active la machine
        /// </summary>
        private void AllumerMachine()
        {
            // Désactiver le Bouton On pour éviter qu'on puisse recliquer dessus et activer le bouton Off
            boutonOn.IsEnabled = false;
            boutonOff.IsEnabled = true;

            _numeroTexte = 0; // Compteur pour le timer pour afficher les textes (selon la valeur de ce compteur)
            afficheurHaut.Text = "Initialisation";

            // Enclencher le timer
            _timerDemarrage.Start();
        }

        /// <summary>
        /// Procédure qui éteint la machine
        /// </summary>
        private void EteindreMachine()
        {
            // Désactiver le Bouton Off pour éviter qu'on puisse recliquer dessus et activer le bouton On
            boutonOff.IsEnabled = false;
            boutonOn.IsEnabled = true;

            ActiverElements(false); // Désactiver les boutons de sélection
            AfficherPieces(Visibility.Visible, Visibility.Collapsed); // Afficher le bouton "Insérer la monnaie", "Rendre la monnaie" et masquer les pièces
            boutonServir.Visibility = Visibility.Collapsed; // Masquer le bouton "Se Servir"
            gobelet2.Visibility = Visibility.Collapsed; // Masquer le gobelet (si le checkbox Gobelet est coché)

            listeRenduMonnaie.Items.Clear(); // Vider la liste du rendu monnaie

            // Remettre à 0 les animations
            glisserGobelet.Stop();
            versement.Stop();
            versementSucre.Stop();

            // Stopper les timers
            _timerDemarrage.Stop();
            _timerRetour.Stop();
            _timerSucre.Stop();

            // Afficher un message pour l'utilisateur
            afficheurHaut.Text = "Hors service";
            afficheurBas.Text = string.Empty;
        }

        /// <summary>
        /// Procédure qui active ou désactiver les boutons de sélection
        /// </summary>
        /// <param name="etat">Nouvel état</param>
        private void ActiverElements(bool etat)
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

        /// <summary>
        /// Procédure qui masque (affiche) le bouton "Insérer la monnaie" et affiche (masque) les pièces
        /// </summary>
        /// <param name="etat1">Visibilité du bouton "Insérer la monnaie"</param>
        /// <param name="etat2">Visibilité des images des pièces et du bouton "Rendre la monnaie"</param>
        private void AfficherPieces(Visibility etat1, Visibility etat2)
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

        /// <summary>
        /// Procédure qui ajoute la monnaie
        /// </summary>
        /// <param name="supplement">La monnaie qui a été ajoutée</param>
        private void AjoutMonnaie(double supplement)
        {
            // Si il y a moins de 2€ inséré, l'ajout de pièces est possible
            if (_monnaie < 2.0)
            {
                // Stopper le timer si l'utilisateur a bougé le scroll du sucre ou coché la case pour le gobelet
                _timerRetour.Stop();

                // Incrémenter la monnaie et l'afficher
                _monnaie += supplement;

                // Afficher le rendu monnaie sur l'afficheur
                afficheurBas.Text = string.Format("Monnaie: {0:C}", _monnaie);
            }
        }

        /// <summary>
        /// Procédure qui verse le liquide dans le gobelet
        /// </summary>
        private void Verser(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            boissonDescend.Fill = brush;
            boissonLever.Fill = brush;

            _timerRetour.Stop();

            // Vider la liste de rendu monnaie
            listeRenduMonnaie.Items.Clear();

            // Vérifier s'il y a assez de monnaie
            if (_monnaie >= 0.4)
            {
                // Désactiver les boutons de sélection
                ActiverElements(false);

                // Affiche le bouton "Insérer la monnaie" et masque les pièces
                AfficherPieces(Visibility.Visible, Visibility.Collapsed);

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
                        _nbSucresVerses = 0;
                        _timerSucre.Start();
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
                afficheurBas.Text = string.Format("Prix: {0:C}", 0.4);
            }
        }

        /// <summary>
        /// Procédure qui rend la monnaie
        /// </summary>
        private void RenduMonnaie()
        {
            // Initialisation des variables qui comptent le nombre de pièces
            int nbPieces2Euros = 0;
            int nbPieces1Euro = 0;
            int nbPieces50Centimes = 0;
            int nbPieces20Centimes = 0;
            int nbPieces10Centimes = 0;
            int nbPieces5Centimes = 0;

            // Afficher le rendu monnaie sur l'afficheur
            afficheurBas.Text = string.Format("Rendu monnaie: {0:C}", _monnaie);

            // Rendre toute la monnaie
            while (_monnaie >= 2)
            {
                _monnaie = _monnaie - 2;
                nbPieces2Euros++;
            }

            while (_monnaie >= 1)
            {
                _monnaie = _monnaie - 1;
                nbPieces1Euro++;
            }

            while (_monnaie >= 0.5)
            {
                _monnaie = _monnaie - 0.5;
                nbPieces50Centimes++;
            }

            while (_monnaie >= 0.2)
            {
                _monnaie = _monnaie - 0.2;
                nbPieces20Centimes++;
            }

            while (_monnaie >= 0.1)
            {
                _monnaie = _monnaie - 0.1;
                nbPieces10Centimes++;
            }

            while (_monnaie >= 0.05)
            {
                _monnaie = _monnaie - 0.05;
                nbPieces5Centimes++;
            }

            // Afficher le nombre de pièces rendues dans la Listbox
            if (nbPieces2Euros > 0)
                listeRenduMonnaie.Items.Add(nbPieces2Euros + " pièces de 2€");

            if (nbPieces1Euro > 0)
                listeRenduMonnaie.Items.Add(nbPieces1Euro + " pièces de 1€");

            if (nbPieces50Centimes > 0)
                listeRenduMonnaie.Items.Add(nbPieces50Centimes + " pièces de 50 centimes");

            if (nbPieces20Centimes > 0)
                listeRenduMonnaie.Items.Add(nbPieces20Centimes + " pièces de 20 centimes");

            if (nbPieces10Centimes > 0)
                listeRenduMonnaie.Items.Add(nbPieces10Centimes + " pièces de 10 centimes");

            if (nbPieces5Centimes > 0)
                listeRenduMonnaie.Items.Add(nbPieces5Centimes + " pièces de 5 centimes");

            _monnaie = 0;

            // Masquer les pièces et afficher les boutons
            AfficherPieces(Visibility.Visible, Visibility.Collapsed);
        }

        #endregion
    }
}