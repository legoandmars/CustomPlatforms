using System;
using System.Linq;

using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;

using HMUI;

using UnityEngine;

using static CustomFloorPlugin.GlobalCollection;


namespace CustomFloorPlugin.UI {


    /// <summary>
    /// A <see cref="ViewController"/> generated and maintained by BSML at runtime.<br/>
    /// BSML uses the <see cref="ResourceName"/> to determine the Layout of the <see cref="GameObject"/>s and their <see cref="Component"/>s<br/>
    /// Tagged functions and variables from this class may be used/called by BSML if the .bsml file mentions them.<br/>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1812:Avoid unistantiated internal classes", Justification = "Instantiated by Unity")]
    internal class PlatformsListView : BSMLResourceViewController {


        /// <summary>
        /// Path to the embedded resource
        /// </summary>
        public override string ResourceName => "CustomFloorPlugin.UI.PlatformList.bsml";


        /// <summary>
        /// Override choice for platform base model/environment<br/>
        /// Forwards the current choice to the UI, and the new choice to the plugin
        /// </summary>
        public static EnvOverrideMode EnvOr {
            get {
                if (_EnvOr == null) {
                    _EnvOr = (EnvOverrideMode)(CONFIG.GetInt("Settings", "EnvironmentOverrideMode", 0, true));
                }
                return _EnvOr.Value;
            }
            set {
                if (value != _EnvOr.Value) {
                    CONFIG.SetInt("Settings", "EnvironmentOverrideMode", (int)value);
                    _EnvOr = value;
                }
            }
        }
        private static EnvOverrideMode? _EnvOr;


        /// <summary>
        /// <see cref="BSMLParserParams"/> used to fire <see cref="UIAction"/>s
        /// </summary>
        [UIParams]
        internal BSMLParserParams parserParams = new BSMLParserParams();


        /// <summary>
        /// The table of currently loaded Platforms
        /// </summary>
        [UIComponent("PlatformsList")]
        public CustomListTableData PlatformListTable = null;

        [UIComponent("OverrideList")]
        public CustomListTableData OverrideListTable = null;


        /// <summary>
        /// Called when a <see cref="CustomPlatform"/> is selected by the user<br/>
        /// Passes the choice to the <see cref="PlatformManager"/>
        /// </summary>
        /// <param name="ignored1">I love how optimised BSML is</param>
        /// <param name="idx">Cell index of the users selection</param>
        [UIAction("PlatformSelect")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Called by BSML")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1801:Review unused parameters", Justification = "BSML")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "BSML")]
        private void PlatformSelect(TableView ignored1, int idx) {
            PlatformManager.SetPlatformAndShow(idx);
        }

        /// <summary>
        /// Called when a <see cref="EnvOverrideMode"/> is selected by the user<br/>
        /// Passes the choice to <see cref="EnvOr"/>
        /// </summary>
        /// <param name="ignored1">I love how optimised BSML is</param>
        /// <param name="idx">Cell index of the users selection</param>
        [UIAction("OverrideSelect")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Called by BSML")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1801:Review unused parameters", Justification = "BSML")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "BSML")]
        private void OverrideSelect(TableView ignored1, int idx) {
            EnvOr = (EnvOverrideMode)idx;
            parserParams.EmitEvent("close-OverrideModal");
        }

        [UIAction("reloadPlatforms")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Called by BSML")]
        private void ReloadMaterials() {
            PlatformManager.Reload();
        }


        /// <summary>
        /// Swapping back to the standard menu environment when the menu is closed<br/>
        /// [Called by Beat Saber]
        /// </summary>
        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling) {
            PlatformManager.ChangeToPlatform(0);
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }


        /// <summary>
        /// Changing to the current platform when the menu is shown<br/>
        /// [Called by Beat Saber]
        /// </summary>
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
            PlatformManager.ChangeToPlatform();
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        }


        /// <summary>
        /// (Re-)Loading the table for the ListView of available platforms and environments to override.<br/>
        /// [Called by BSML]
        /// </summary>
        [UIAction("#post-parse")]
        internal void SetupLists() {
            SetupPlatformsList();
            SetupOverrideList();
        }

        private void SetupPlatformsList() {
            PlatformListTable.data.Clear();
            foreach (CustomPlatform platform in PlatformManager.AllPlatforms) {
                PlatformListTable.data.Add(new CustomListTableData.CustomCellInfo(platform.platName, platform.platAuthor, platform.icon));
            }
            PlatformListTable.tableView.ReloadData();
            int selectedPlatform = PlatformManager.CurrentPlatformIndex;
            if (!PlatformListTable.tableView.visibleCells.Any(x => x.selected)) {
                PlatformListTable.tableView.ScrollToCellWithIdx(selectedPlatform, TableViewScroller.ScrollPositionType.Beginning, false);
            }

            PlatformListTable.tableView.SelectCellWithIdx(selectedPlatform);
        }

        private void SetupOverrideList() {
            OverrideListTable.data.Clear();
            foreach (string name in Enum.GetNames(typeof(EnvOverrideMode))) {
                OverrideListTable.data.Add(new CustomListTableData.CustomCellInfo(name));
            }
            OverrideListTable.tableView.ReloadData();
            int selectedOverride = (int)EnvOr;
            if (!OverrideListTable.tableView.visibleCells.Any(x => x.selected)) {
                OverrideListTable.tableView.ScrollToCellWithIdx(selectedOverride, TableViewScroller.ScrollPositionType.Beginning, false);
            }

            OverrideListTable.tableView.SelectCellWithIdx(selectedOverride);
        }
    }
}