using ExitGames.Logging;
using MongoDB.Bson;
using Space.Game;
using Space.Game.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Database {
    public class ShipModelDocument  {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ObjectId Id { get; set; }

        public string CharacterId { get; set; }

        public ShipModuleDocumentElement ES { get; set; }
        public ShipModuleDocumentElement CB { get; set; }
        public ShipModuleDocumentElement DF { get; set; }
        public ShipModuleDocumentElement CM { get; set; }
        public ShipModuleDocumentElement DM { get; set; }

        public bool IsNewDocument { get; set; }


        public void Set(ShipModel sourceObject) {
            if(sourceObject == null ) {
                return;
            }
            ValidateProperties();
            this.ES.Set(sourceObject.ES.Module);
            this.CB.Set(sourceObject.CB.Module);
            this.DF.Set(sourceObject.DF.Module);
            this.CM.Set(sourceObject.CM.Module);
            this.DM.Set(sourceObject.DM.Module);
            IsNewDocument = false;
        }

        public ShipModel SourceObject(IRes resource) {
            var model = new ShipModel(resource);
            ShipModule prevModule = null;
            try {
                if (this.ES != null)
                    model.ES.SetModule(this.ES.SourceObject(), out prevModule);
                if (this.CB != null)
                    model.CB.SetModule(this.CB.SourceObject(), out prevModule);
                if (this.DF != null)
                    model.DF.SetModule(this.DF.SourceObject(), out prevModule);
                if (this.CM != null)
                    model.CM.SetModule(this.CM.SourceObject(), out prevModule);
                if (this.DM != null)
                    model.DM.SetModule(this.DM.SourceObject(), out prevModule);
            } catch (Exception ex) {
                log.Error(ex);
                log.Error(ex.StackTrace);
            }
            return model;
        }

        private void ValidateProperties() {
            if (this.ES == null)
                this.ES = new ShipModuleDocumentElement();
            if (this.CB == null)
                this.CB = new ShipModuleDocumentElement();
            if (this.DF == null)
                this.DF = new ShipModuleDocumentElement();
            if (this.CM == null)
                this.CM = new ShipModuleDocumentElement();
            if (this.DM == null)
                this.DM = new ShipModuleDocumentElement();
        }
    }
}
