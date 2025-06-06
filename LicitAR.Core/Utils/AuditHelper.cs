﻿using LicitAR.Core.Data.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Utils
{
    public static class AuditHelper
    {
        public static AuditTable GetCreationData(int IdUsuario)
        {
            return new AuditTable
            {
                FechaAlta = DateTime.UtcNow,
                IdUsuarioAlta = IdUsuario,
                FechaModificacion = null,
                IdUsuarioModificacion = null,
                FechaBaja = null,
                IdUsuarioBaja = null

            };


        }

        public static AuditTable SetModificationData(AuditTable audit, int IdUsuario)
        {
            audit.FechaModificacion = DateTime.UtcNow;
            audit.IdUsuarioModificacion = IdUsuario;

            return audit;
        }

        public static AuditTable SetDeletionData(AuditTable audit, int IdUsuario)
        {
            audit.FechaBaja = DateTime.UtcNow;
            audit.IdUsuarioBaja = IdUsuario;

            return audit;
        }
    }
}
