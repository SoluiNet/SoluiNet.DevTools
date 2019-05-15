namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class TimeTrackingContext : DbContext
    {
        // Der Kontext wurde für die Verwendung einer TimeTrackingContext-Verbindungszeichenfolge aus der 
        // Konfigurationsdatei ('App.config' oder 'Web.config') der Anwendung konfiguriert. Diese Verbindungszeichenfolge hat standardmäßig die 
        // Datenbank 'SoluiNet.DevTools.Utils.TimeTracking.Entities.TimeTrackingContext' auf der LocalDb-Instanz als Ziel. 
        // 
        // Wenn Sie eine andere Datenbank und/oder einen anderen Anbieter als Ziel verwenden möchten, ändern Sie die TimeTrackingContext-Zeichenfolge 
        // in der Anwendungskonfigurationsdatei.
        public TimeTrackingContext()
            : base("name=TimeTrackingContext")
        {
        }

        // Fügen Sie ein 'DbSet' für jeden Entitätstyp hinzu, den Sie in das Modell einschließen möchten. Weitere Informationen 
        // zum Konfigurieren und Verwenden eines Code First-Modells finden Sie unter 'http://go.microsoft.com/fwlink/?LinkId=390109'.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<UsageTime> UsageTimes { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}