using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestorEncuestas_MVC.Models;
using Microsoft.AspNetCore.Identity;

namespace GestorEncuestas_MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario, Rol, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Encuesta> Encuestas { get; set; }
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<PreguntaOpcion> PreguntasOpciones { get; set; }
        public DbSet<Respuesta> Respuestas { get; set; }
        public DbSet<RespuestaOpcion> RespuestasOpciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar nombres de tablas para Identity (personalizadas)
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Rol>().ToTable("roles");
            
            // Configurar nombres de tablas para las tablas de Identity
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("user_claims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("user_logins");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("user_tokens");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("role_claims");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("user_roles");

            // Configurar mapeo de columnas para Usuario (Identity)
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Id)
                .HasColumnName("id");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.UserName)
                .HasColumnName("username");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.NormalizedUserName)
                .HasColumnName("normalized_username");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.PasswordHash)
                .HasColumnName("passwd");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.RolId)
                .HasColumnName("rol");

            // Mapear propiedades de Identity a las columnas correctas
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Email)
                .HasColumnName("email");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.NormalizedEmail)
                .HasColumnName("normalized_email");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.EmailConfirmed)
                .HasColumnName("email_confirmed");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.PhoneNumber)
                .HasColumnName("phone_number");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.PhoneNumberConfirmed)
                .HasColumnName("phone_number_confirmed");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.TwoFactorEnabled)
                .HasColumnName("two_factor_enabled");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.LockoutEnd)
                .HasColumnName("lockout_end");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.LockoutEnabled)
                .HasColumnName("lockout_enabled");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.AccessFailedCount)
                .HasColumnName("access_failed_count");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.ConcurrencyStamp)
                .HasColumnName("concurrency_stamp");
                
            modelBuilder.Entity<Usuario>()
                .Property(u => u.SecurityStamp)
                .HasColumnName("security_stamp");

            // Configurar mapeo de columnas para Rol (Identity)
            modelBuilder.Entity<Rol>()
                .Property(r => r.Id)
                .HasColumnName("id");
                
            modelBuilder.Entity<Rol>()
                .Property(r => r.DisplayRolNombre)
                .HasColumnName("rol");
                
            // Mapear propiedades esenciales de Identity para Roles
            modelBuilder.Entity<Rol>()
                .Property(r => r.Name)
                .HasColumnName("name");
                
            modelBuilder.Entity<Rol>()
                .Property(r => r.NormalizedName)
                .HasColumnName("normalized_name");
                
            modelBuilder.Entity<Rol>()
                .Property(r => r.ConcurrencyStamp)
                .HasColumnName("concurrency_stamp");

            // Configurar mapeo de columnas para Encuesta
            modelBuilder.Entity<Encuesta>()
                .Property(e => e.Id)
                .HasColumnName("id");
                
            modelBuilder.Entity<Encuesta>()
                .Property(e => e.AutorId)
                .HasColumnName("autor");
                
            modelBuilder.Entity<Encuesta>()
                .Property(e => e.Titulo)
                .HasColumnName("titulo");
                
            modelBuilder.Entity<Encuesta>()
                .Property(e => e.Descripcion)
                .HasColumnName("descripcion");
                
            modelBuilder.Entity<Encuesta>()
                .Property(e => e.Estado)
                .HasColumnName("estado");
                
            modelBuilder.Entity<Encuesta>()
                .Property(e => e.CierraEn)
                .HasColumnName("cierra_en");
                
            modelBuilder.Entity<Encuesta>()
                .Property(e => e.CreadoEn)
                .HasColumnName("creado_en");

            // Configurar mapeo de columnas para Pregunta
            modelBuilder.Entity<Pregunta>()
                .Property(p => p.Id)
                .HasColumnName("id");
                
            modelBuilder.Entity<Pregunta>()
                .Property(p => p.EncuestaId)
                .HasColumnName("encuesta_id");
                
            modelBuilder.Entity<Pregunta>()
                .Property(p => p.Enunciado)
                .HasColumnName("enunciado");
                
            modelBuilder.Entity<Pregunta>()
                .Property(p => p.TipoPregunta)
                .HasColumnName("tipo_pregunta");
                
            modelBuilder.Entity<Pregunta>()
                .Property(p => p.Obligatorio)
                .HasColumnName("obligatorio");

            // Configurar mapeo de columnas para PreguntaOpcion
            modelBuilder.Entity<PreguntaOpcion>()
                .Property(po => po.Id)
                .HasColumnName("id");
                
            modelBuilder.Entity<PreguntaOpcion>()
                .Property(po => po.PreguntaId)
                .HasColumnName("pregunta_id");
                
            modelBuilder.Entity<PreguntaOpcion>()
                .Property(po => po.Position)
                .HasColumnName("position");
                
            modelBuilder.Entity<PreguntaOpcion>()
                .Property(po => po.Label)
                .HasColumnName("Label");
                
            modelBuilder.Entity<PreguntaOpcion>()
                .Property(po => po.Value)
                .HasColumnName("Value");

            // Configurar mapeo de columnas para Respuesta
            modelBuilder.Entity<Respuesta>()
                .Property(r => r.Id)
                .HasColumnName("id");
                
            modelBuilder.Entity<Respuesta>()
                .Property(r => r.UsuarioId)
                .HasColumnName("usuario_respuesta");
                
            modelBuilder.Entity<Respuesta>()
                .Property(r => r.EncuestaId)
                .HasColumnName("encuesta_id");
                
            modelBuilder.Entity<Respuesta>()
                .Property(r => r.PreguntaId)
                .HasColumnName("pregunta_id");
                
            modelBuilder.Entity<Respuesta>()
                .Property(r => r.Texto)
                .HasColumnName("respuesta");
                
            modelBuilder.Entity<Respuesta>()
                .Property(r => r.Numerica)
                .HasColumnName("respuesta_numeros");
                
            modelBuilder.Entity<Respuesta>()
                .Property(r => r.FechaRespuesta)
                .HasColumnName("fecha_respuesta");
                
            modelBuilder.Entity<Respuesta>()
                .Property(r => r.SeleccionOpcionId)
                .HasColumnName("seleccion_opcion_id");

            // Configurar mapeo de columnas para RespuestaOpcion
            modelBuilder.Entity<RespuestaOpcion>()
                .Property(ro => ro.RespuestaId)
                .HasColumnName("respuesta_id");
                
            modelBuilder.Entity<RespuestaOpcion>()
                .Property(ro => ro.OpcionId)
                .HasColumnName("opcion");

            // Configurar la tabla intermedia RespuestaOpcion (N:M)
            modelBuilder.Entity<RespuestaOpcion>()
                .HasKey(ro => new { ro.RespuestaId, ro.OpcionId });

            modelBuilder.Entity<RespuestaOpcion>()
                .HasOne(ro => ro.Respuesta)
                .WithMany(r => r.RespuestasOpciones)
                .HasForeignKey(ro => ro.RespuestaId);

            modelBuilder.Entity<RespuestaOpcion>()
                .HasOne(ro => ro.Opcion)
                .WithMany(o => o.RespuestasOpciones)
                .HasForeignKey(ro => ro.OpcionId);

            // Configurar relaciones adicionales
            modelBuilder.Entity<Encuesta>()
                .HasOne(e => e.Autor)
                .WithMany(u => u.EncuestasCreadas)
                .HasForeignKey(e => e.AutorId);

            modelBuilder.Entity<Pregunta>()
                .HasOne(p => p.Encuesta)
                .WithMany(e => e.Preguntas)
                .HasForeignKey(p => p.EncuestaId);

            modelBuilder.Entity<PreguntaOpcion>()
                .HasOne(po => po.Pregunta)
                .WithMany(p => p.Opciones)
                .HasForeignKey(po => po.PreguntaId);

            modelBuilder.Entity<Respuesta>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Respuestas)
                .HasForeignKey(r => r.UsuarioId);

            modelBuilder.Entity<Respuesta>()
                .HasOne(r => r.Encuesta)
                .WithMany(e => e.Respuestas)
                .HasForeignKey(r => r.EncuestaId);

            modelBuilder.Entity<Respuesta>()
                .HasOne(r => r.Pregunta)
                .WithMany(p => p.Respuestas)
                .HasForeignKey(r => r.PreguntaId);

            modelBuilder.Entity<Respuesta>()
                .HasOne(r => r.SeleccionOpcion)
                .WithMany(po => po.Respuestas)
                .HasForeignKey(r => r.SeleccionOpcionId);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId);
        }
    }
}