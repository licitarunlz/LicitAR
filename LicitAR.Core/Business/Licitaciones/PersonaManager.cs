using LicitAR.Core.Data.Models;
using LicitAR.Core.Data;
using LicitAR.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LicitAR.Core.Data.Models.Helpers;

namespace LicitAR.Core.Business.Licitaciones
{
    public interface IPersonaManager
    {
        Task<IMessageManager> AgregarAsync(Persona persona, int idUser);
        Task<IMessageManager> BajaLogicaAsync(int idPersona, int idUser);
        Task<IEnumerable<Persona>> GetAllPersonasAsync();
        Task<Persona?> GetPersonaByIdAsync(int idPersona);
        Task<IMessageManager> ModificarAsync(Persona persona, int idPersona, int idUser);

        Task<IMessageManager> AsociarUsuarioAsync(Persona persona, string user,  int idUser);

        Task<PersonaUsuario> GetPersonaAsociadaAsync(string user);
    }

    public class PersonaManager : IPersonaManager
    {
        private IMessageManager _messageManager;
        private readonly LicitARDbContext _dbContext;

        public PersonaManager(IMessageManager messageManager, LicitARDbContext dbContext)
        {
            this._messageManager = messageManager;
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<Persona>> GetAllPersonasAsync()
        {
            return this._dbContext.Personas
                                    .Include(p => p.Provincia)
                                    .Include(p => p.Localidad)
                                    .Include(p => p.TipoPersona)
                                    .Include(p => p.Rubro)
                                    .Where(x => x.Audit.FechaBaja == null).ToList();
        }
        public async Task<Persona?> GetPersonaByIdAsync(int idPersona)
        {
            return await this._dbContext.Personas
                                .Include(p => p.Provincia)
                                .Include(p => p.Localidad)
                                .Include(p => p.TipoPersona)
                                .Include(p => p.Rubro).FirstOrDefaultAsync(x => x.IdPersona == idPersona);
        }
        public async Task<IMessageManager> AgregarAsync(Persona persona, int idUser)
        {
            try
            {
                persona.Audit = AuditHelper.GetCreationData(idUser);
                _dbContext.Personas.Add(persona);
                await _dbContext.SaveChangesAsync();

                _messageManager.OkMessage("Persona agregada exitosamente!");
            }
            catch (Exception ex)
            {
                _messageManager.ClearMessages();
                _messageManager.ErrorMessage("Error al intentar actualizar la base de datos " + ex.ToString());
            }

            return _messageManager;
        }
        public async Task<IMessageManager> ModificarAsync(Persona persona, int idPersona, int idUser)
        {
            try
            {
                var entidadFromDdbb = await _dbContext.Personas.FirstOrDefaultAsync(x => x.IdPersona == idPersona);
                if (entidadFromDdbb == null)
                {
                    _messageManager.ErrorMessage("Persona no encontrada");
                }
                else
                {
                    entidadFromDdbb.Email = persona.Email;
                    entidadFromDdbb.Telefono = persona.Telefono;
                    entidadFromDdbb.Cuit = persona.Cuit;
                    entidadFromDdbb.IdTipoPersona = persona.IdTipoPersona;
                    entidadFromDdbb.RazonSocial = persona.RazonSocial;
                    entidadFromDdbb.IdLocalidad = persona.IdLocalidad;
                    entidadFromDdbb.IdProvincia = persona.IdProvincia;
                    entidadFromDdbb.DireccionBarrio = persona.DireccionBarrio;
                    entidadFromDdbb.DireccionCalle = persona.DireccionCalle;
                    entidadFromDdbb.DireccionNumero = persona.DireccionNumero;
                    entidadFromDdbb.DireccionPiso = persona.DireccionPiso;
                    entidadFromDdbb.DireccionDepto = persona.DireccionDepto;
                    entidadFromDdbb.DireccionCodigoPostal = persona.DireccionCodigoPostal;
                    entidadFromDdbb.IdRubro = persona.IdRubro;
                    entidadFromDdbb.Audit = AuditHelper.SetModificationData(entidadFromDdbb.Audit, idUser);

                    _dbContext.Personas.Update(entidadFromDdbb);

                    await _dbContext.SaveChangesAsync();

                    _messageManager.OkMessage("Persona modificada exitosamente");


                }
            }
            catch (Exception ex)
            {
                _messageManager.ClearMessages();
                _messageManager.ErrorMessage("Excepcion al modificar la entidad licitante: Ex - " + ex.ToString());
            }
            return _messageManager;


        }
        public async Task<IMessageManager> BajaLogicaAsync(int idPersona, int idUser)
        {
            try
            {
                var persona = await _dbContext.Personas.FirstOrDefaultAsync(x => x.IdPersona == idPersona);
                if (persona == null)
                {
                    _messageManager.ErrorMessage("Persona no encontrada");
                }
                else
                {
                    persona.Audit = AuditHelper.SetDeletionData(persona.Audit, idUser);

                    _dbContext.Personas.Update(persona);
                    await _dbContext.SaveChangesAsync();

                    _messageManager.OkMessage("Persona eliminada exitosamente");


                }
            }
            catch (Exception ex)
            {
                _messageManager.ClearMessages();
                _messageManager.ErrorMessage("Excepcion al eliminar la entidad licitante: Ex - " + ex.ToString());

            }
            return _messageManager;
        }

        public async Task<IMessageManager> AsociarUsuarioAsync(Persona persona, string user, int idUser)
        {

            try
            {
                AuditTable audit = AuditHelper.GetCreationData(idUser);
                PersonaUsuario vinculacion = new PersonaUsuario 
                { 
                    Audit = audit 
                };

                vinculacion.IdPersona = persona.IdPersona;
                vinculacion.IdUsuario = user;

                _dbContext.PersonaUsuarios.Add(vinculacion);

                await _dbContext.SaveChangesAsync();


                _messageManager.OkMessage("Persona vinculada exitosamente");
                _messageManager.addedData = vinculacion;

            }
            catch (Exception ex)
            {
                _messageManager.ClearMessages();
                _messageManager.ErrorMessage("Excepcion al vincular al usuario con la persona: Ex - " + ex.ToString());

            }
            return _messageManager;
        }

        public async Task<PersonaUsuario> GetPersonaAsociadaAsync(string user)
        {
            try
            {
                PersonaUsuario? vinculacion = await _dbContext.PersonaUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == user);

                return vinculacion;

            }
            catch
            {
                throw;
            }

        }

    }
}
