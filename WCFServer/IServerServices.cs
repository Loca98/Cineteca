﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServer
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di interfaccia "IServerServices" nel codice e nel file di configurazione contemporaneamente.
    [ServiceContract]
    public interface IServerServices
    {
        [OperationContract]
        bool DoWork();

        [OperationContract]
        bool NuovoUtente(int myId, string myNome, string myCognome, string myEmail, string myPassword, string myIsAdmin);
    }
}
