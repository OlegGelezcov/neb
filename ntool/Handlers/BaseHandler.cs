using Common;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;


namespace ntool.Handlers {
    public abstract class BaseHandler {

        private byte m_Code;
        private Application m_Application;

        public BaseHandler(byte code, Application context) {
            m_Code = code;
            m_Application = context;
        }

        public Application app {
            get {
                return m_Application;
            }
        }

        public byte code {
            get {
                return m_Code;
            }
        }

        public abstract void Handle(OperationResponse response);

        protected virtual void LogResponse(OperationResponse response) {
            app.logger.PushColor(ConsoleColor.Cyan);
            app.logger.Log("Operation: {0}", (OperationCode)response.OperationCode);
            app.logger.Log("ReturnCode: {0}", (ReturnCode)response.ReturnCode);
            app.logger.Log("DebugMessage: {0}", response.DebugMessage);

            string parametersString = "(none)";
            if(response.Parameters != null ) {
                Dictionary<ParameterCode, object> readableParameters = new Dictionary<ParameterCode, object>();
                foreach(var kvp in response.Parameters) {
                    readableParameters.Add((ParameterCode)kvp.Key, kvp.Value);
                }
                parametersString = readableParameters.toHash().ToStringBuilder().ToString();

            }
            app.logger.Log("Parameters -> ");
            app.logger.Log("{0}", parametersString);
            app.logger.PopColor();

        }
    }
}
