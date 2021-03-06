﻿using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.MiddleTier {
    public class XpandServerApplication : ServerApplication, ISupportFullConnectionString, IXafApplication {
        ApplicationModulesManager _applicationModulesManager;
        string ISupportFullConnectionString.ConnectionString { get; set; }

        IDataStore IXafApplication.GetDataStore(IDataStore dataStore) {
            return null;
        }


        public new string ConnectionString {
            get { return base.ConnectionString; }
            set {
                base.ConnectionString = value;
                ((ISupportFullConnectionString)this).ConnectionString = value;
            }
        }

        string IXafApplication.RaiseEstablishingConnection() {
            return this.GetConnectionString();
        }

        protected override ApplicationModulesManager CreateApplicationModulesManager(ControllersManager controllersManager) {
            _applicationModulesManager = base.CreateApplicationModulesManager(controllersManager);
            return _applicationModulesManager;
        }

        ApplicationModulesManager IXafApplication.ApplicationModulesManager {
            get { return _applicationModulesManager; }
        }
        public event EventHandler UserDifferencesLoaded;

        protected virtual void OnUserDifferencesLoaded(EventArgs e) {
            EventHandler handler = UserDifferencesLoaded;
            if (handler != null) handler(this, e);
        }

    }
}
