﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HADES {
    using System;
    
    
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HADES.App_GlobalResources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Paramètres de l&apos;application.
        /// </summary>
        public static string AppConfig {
            get {
                return ResourceManager.GetString("AppConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Retour.
        /// </summary>
        public static string Back {
            get {
                return ResourceManager.GetString("Back", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Abandonner.
        /// </summary>
        public static string Cancel {
            get {
                return ResourceManager.GetString("Cancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Supprimer.
        /// </summary>
        public static string Delete {
            get {
                return ResourceManager.GetString("Delete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à L’utilisation du champ de la date d’expiration pour les groupes nécessite une configuration spéciale. Pour que le champ de la date d’expiration des groupes soit fonctionnel, il faut ajouter un attribut personnalisé et l’ajouter à la classe groupe de votre Active Directory. Ainsi, cette information ne sera pas conservée dans la base de données d’Hades, mais bien directement, dans votre Active Directory. .
        /// </summary>
        public static string doc_notexp01 {
            get {
                return ResourceManager.GetString("doc_notexp01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Sur l&apos;outil d&apos;administration du schéma Active Directory, accédez à Classes, sélectionnez group puis accédez à ses propriétés..
        /// </summary>
        public static string doc_notexp010 {
            get {
                return ResourceManager.GetString("doc_notexp010", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Allez dans l&apos;onglet Attributs, cliquez sur Ajouter, puis ajoutez dateDexpirationHades Une fois terminé, sélectionnez l&apos;attribut à ajouter et cliquez sur OK. Cliquer sur Appliquer..
        /// </summary>
        public static string doc_notexp011 {
            get {
                return ResourceManager.GetString("doc_notexp011", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Pour que la nouvelle attribut soit disponible dans votre Active Directory, vous devez redémarrez votre serveur. Pour valider que votre nouvelle attribut est disponible, vous pouvez aller voir dans l&apos;éditeur d&apos;attributes d&apos;un groupe dans votre Active Directory..
        /// </summary>
        public static string doc_notexp012 {
            get {
                return ResourceManager.GetString("doc_notexp012", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Voici un petit guide étape par étape pour vous aider à réaliser cette configuration..
        /// </summary>
        public static string doc_notexp02 {
            get {
                return ResourceManager.GetString("doc_notexp02", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Pour commencer il faut installer le composant logiciel de schéma Active Directory. Vous devez exécuter la commande regsvr32 schmmgmt.dll à l&apos;aide d&apos;une invite de commande élevée (en utilisant l&apos;option Exécuter en tant qu&apos;administrateur)..
        /// </summary>
        public static string doc_notexp03 {
            get {
                return ResourceManager.GetString("doc_notexp03", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Une fois l&apos;outil d&apos;administration du schéma Active Directory installé, il sera disponible sur MMC..
        /// </summary>
        public static string doc_notexp04 {
            get {
                return ResourceManager.GetString("doc_notexp04", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Ensuite, vous devez ajouter le Schéma Active Directory comme Snap-in. Cliquer sur Fichier et sur Ajouter/Supprimer des Snap-in....
        /// </summary>
        public static string doc_notexp05 {
            get {
                return ResourceManager.GetString("doc_notexp05", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Sélectionnez Active Directory Schéma et sélectionnez ajouter. Cliquer sur Ok..
        /// </summary>
        public static string doc_notexp06 {
            get {
                return ResourceManager.GetString("doc_notexp06", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Dans l&apos;outil d&apos;administration du schéma Active Directory, faites un clic droit sur Attributs, puis sélectionnez Créer un attribut… .
        /// </summary>
        public static string doc_notexp07 {
            get {
                return ResourceManager.GetString("doc_notexp07", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Cliquez sur Continuer (l&apos;avertissement qui s&apos;affiche est pour informer que la création d&apos;un nouvel attribut Active Directory n&apos;est pas une opération réversible et qu&apos;il ne peut pas être supprimé une fois terminé).
        /// </summary>
        public static string doc_notexp08 {
            get {
                return ResourceManager.GetString("doc_notexp08", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Renseignez les informations suivantes. Il est très important que les informations soient écrites exactement de cette façon. Le nom commun est ExpirationDateHades. Le nom de LDAP est expirationDateHades. Pour le Object ID, vous devez en générer un. Vous trouverez un lien ci-dessous vers un générateur, mais n&apos;importe quel générateur peux faire l&apos;affaire. La syntaxe est UTC Coded Time. Le restant des champs sont optionnels..
        /// </summary>
        public static string doc_notexp09 {
            get {
                return ResourceManager.GetString("doc_notexp09", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à La date d&apos;expiration des groupes.
        /// </summary>
        public static string doc_notexpdateTitle {
            get {
                return ResourceManager.GetString("doc_notexpdateTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Documentation.
        /// </summary>
        public static string Documentation {
            get {
                return ResourceManager.GetString("Documentation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à La date d&apos;expiration est arrivée a échéance pour un groupe..
        /// </summary>
        public static string email_ExpirationDateMessage {
            get {
                return ResourceManager.GetString("email_ExpirationDateMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Le groupe expirera dans.
        /// </summary>
        public static string email_ExpirationDateSoonMsg {
            get {
                return ResourceManager.GetString("email_ExpirationDateSoonMsg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à jour(s)..
        /// </summary>
        public static string email_ExpirationDateSoonMsg02 {
            get {
                return ResourceManager.GetString("email_ExpirationDateSoonMsg02", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à La date d&apos;expiration.
        /// </summary>
        public static string email_ExpirationDateSubject {
            get {
                return ResourceManager.GetString("email_ExpirationDateSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Un groupe a été créé..
        /// </summary>
        public static string email_GroupCreateMessage {
            get {
                return ResourceManager.GetString("email_GroupCreateMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à La création d&apos;un groupe.
        /// </summary>
        public static string email_GroupCreateSubject {
            get {
                return ResourceManager.GetString("email_GroupCreateSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Un groupe a été supprimé..
        /// </summary>
        public static string email_GroupDeleteMessage {
            get {
                return ResourceManager.GetString("email_GroupDeleteMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à La suppression d&apos;un groupe.
        /// </summary>
        public static string email_GroupDeleteSubject {
            get {
                return ResourceManager.GetString("email_GroupDeleteSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Des membres ont été ajouté à un groupe..
        /// </summary>
        public static string email_MemberAddMessage {
            get {
                return ResourceManager.GetString("email_MemberAddMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Ajout de Membres.
        /// </summary>
        public static string email_MemberAddSubject {
            get {
                return ResourceManager.GetString("email_MemberAddSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Des membres ont été supprimé d&apos;un groupe..
        /// </summary>
        public static string email_MemberRemovalMessage {
            get {
                return ResourceManager.GetString("email_MemberRemovalMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Suppression de membres.
        /// </summary>
        public static string email_MemberRemovalSubject {
            get {
                return ResourceManager.GetString("email_MemberRemovalSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Membres:.
        /// </summary>
        public static string email_Members {
            get {
                return ResourceManager.GetString("email_Members", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Membres ajoutés: .
        /// </summary>
        public static string email_membersaddes {
            get {
                return ResourceManager.GetString("email_membersaddes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Membres supprimés: .
        /// </summary>
        public static string email_membersSup {
            get {
                return ResourceManager.GetString("email_membersSup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Nom:.
        /// </summary>
        public static string email_name {
            get {
                return ResourceManager.GetString("email_name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Anglais.
        /// </summary>
        public static string English {
            get {
                return ResourceManager.GetString("English", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Un fichier est trop grand.
        /// </summary>
        public static string errorFileSize {
            get {
                return ResourceManager.GetString("errorFileSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à La configuration n&apos;a pas pu être sauvegardée vérifier que les champs sont bien remplis.
        /// </summary>
        public static string ErrorSavingConfiguration {
            get {
                return ResourceManager.GetString("ErrorSavingConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Français.
        /// </summary>
        public static string French {
            get {
                return ResourceManager.GetString("French", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à GlobalFR.
        /// </summary>
        public static string GlobalString {
            get {
                return ResourceManager.GetString("GlobalString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Lien vers le Genérateur.
        /// </summary>
        public static string link_generator {
            get {
                return ResourceManager.GetString("link_generator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Dossiers et groupes.
        /// </summary>
        public static string MainView {
            get {
                return ResourceManager.GetString("MainView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Une valeur négative à été entrée alors que l&apos;application attendaient une valeur positive.
        /// </summary>
        public static string NegativeValueError {
            get {
                return ResourceManager.GetString("NegativeValueError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à OK.
        /// </summary>
        public static string OK {
            get {
                return ResourceManager.GetString("OK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Portugais.
        /// </summary>
        public static string Portuguese {
            get {
                return ResourceManager.GetString("Portuguese", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Retirer.
        /// </summary>
        public static string Remove {
            get {
                return ResourceManager.GetString("Remove", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Sauvegarder.
        /// </summary>
        public static string Save {
            get {
                return ResourceManager.GetString("Save", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Espagnol.
        /// </summary>
        public static string Spanish {
            get {
                return ResourceManager.GetString("Spanish", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Paramètres de l&apos;utilisateur.
        /// </summary>
        public static string UserConfig {
            get {
                return ResourceManager.GetString("UserConfig", resourceCulture);
            }
        }
    }
}
