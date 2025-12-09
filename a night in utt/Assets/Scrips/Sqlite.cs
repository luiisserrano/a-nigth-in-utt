using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine;

public class Sqlite : MonoBehaviour
{
    public static Sqlite instance;
    private string dbName;

    private void Awake()
    {
        // Patrón Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            dbName = "URI=file:" + Path.Combine(Application.persistentDataPath, "DBGame.db");
            Debug.Log("Ruta de la Base de Datos: " + dbName);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CreateTables();
        InsertarDatosIniciales();
        Debug.Log("Sqlite inicializado correctamente. Esperando acción del jugador...");
    }

    // --- CREACIÓN DE TABLAS ---
    private void CreateTables()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                string sqlcreation = "";
                
                // Unidad 1
                sqlcreation += "CREATE TABLE IF NOT EXISTS desafios (id INTEGER PRIMARY KEY AUTOINCREMENT, Desafio TEXT NOT NULL, respuesta TEXT NOT NULL);";
                // Unidad 2
                sqlcreation += "CREATE TABLE IF NOT EXISTS desafios2 (id INTEGER PRIMARY KEY AUTOINCREMENT, Desafio TEXT NOT NULL, respuesta TEXT NOT NULL);";
                // Unidad 3
                sqlcreation += "CREATE TABLE IF NOT EXISTS desafios3 (id INTEGER PRIMARY KEY AUTOINCREMENT, Desafio TEXT NOT NULL, respuesta TEXT NOT NULL);";
                // Items
                sqlcreation += "CREATE TABLE IF NOT EXISTS items (id INTEGER PRIMARY KEY AUTOINCREMENT, item TEXT NOT NULL, activo INTEGER NOT NULL);";

                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        Debug.Log("Tablas 'desafios', 'desafios2', 'desafios3' e 'items' verificadas/creadas correctamente.");
    }

    // --- INSERCIÓN DE DATOS INICIALES ---
    private void InsertarDatosIniciales()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            // -----------------------
            // Unidad 1
            // -----------------------
            using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM desafios";
                if ((long)checkCmd.ExecuteScalar() == 0)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                INSERT INTO desafios (Desafio, Respuesta) VALUES
                                ('P1U1','R1'),('P2U1','R2'),('P3U1','R1'),('P4U1','R3'),
                                ('P5U1','R2'),('P6U1','R2'),('P7U1','R3'),('P8U1','R1'),
                                ('P9U1','R2'),('P10U1','R3'),('P11U1','R2'),('P12U1','R3'),
                                ('P13U1','R2'),('P14U1','R1');";
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    Debug.Log("Tabla 'desafios' (Unidad 1) creada y poblada correctamente.");
                }
            }

            // -----------------------
            // Unidad 2
            // -----------------------
            using (var checkCmd2 = connection.CreateCommand())
            {
                checkCmd2.CommandText = "SELECT COUNT(*) FROM desafios2";
                if ((long)checkCmd2.ExecuteScalar() == 0)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                INSERT INTO desafios2 (Desafio, Respuesta) VALUES
                                ('P1U2','R1'),('P2U2','R1'),('P3U2','R3'),('P4U2','R3'),
                                ('P5U2','R1'),('P6U2','R2'),('P7U2','R1'),('P8U2','R3'),
                                ('P9U2','R2'),('P10U2','R3'),('P11U2','R3'),('P12U2','R2'),
                                ('P13U2','R1'),('P14U2','R3');";
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    Debug.Log("Tabla 'desafios2' (Unidad 2) creada y poblada correctamente.");
                }
            }

            // -----------------------
            // Unidad 3
            // -----------------------
            using (var checkCmd3 = connection.CreateCommand())
            {
                checkCmd3.CommandText = "SELECT COUNT(*) FROM desafios3";
                if ((long)checkCmd3.ExecuteScalar() == 0)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                INSERT INTO desafios3 (Desafio, Respuesta) VALUES
                                ('P1U3','R1'),('P2U3','R3'),('P3U3','R2'),('P4U3','R2'),
                                ('P5U3','R1'),('P6U3','R1'),('P7U3','R3'),('P8U3','R1'),
                                ('P9U3','R2'),('P10U3','R3'),('P11U3','R2'),('P12U3','R1'),
                                ('P13U3','R2'),('P14U3','R1');";
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    Debug.Log("Tabla 'desafios3' (Unidad 3) creada y poblada correctamente.");
                }
            }

            // -----------------------
            // Items
            // -----------------------
            using (var checkCmdItems = connection.CreateCommand())
            {
                checkCmdItems.CommandText = "SELECT COUNT(*) FROM items";
                if ((long)checkCmdItems.ExecuteScalar() == 0)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                INSERT INTO items (item, activo) VALUES
                                ('Item1',0),('Item2',0),('Item3',0),('Item4',0),
                                ('Item5',0),('Item6',0),('Item7',0),('Item8',0),
                                ('Item9',0),('Item10',0),('Item11',0),('Item12',0);";
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    Debug.Log("Tabla 'items' creada y poblada correctamente.");
                }
            }

            connection.Close();
        }
    }

    // --- MÉTODOS GENERALES ---
    public DataTable EjecutarConsulta(string sql)
    {
        DataTable dt = new DataTable();
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                using (IDataReader reader = command.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
            connection.Close();
        }
        return dt;
    }

    public DataRow ObtenerDesafioPorIndex(int index)
    {
        string sql = "SELECT * FROM desafios LIMIT 1 OFFSET " + index;
        DataTable dt = EjecutarConsulta(sql);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public DataRow ObtenerDesafioPorIndexUnidad2(int index)
    {
        string sql = "SELECT * FROM desafios2 LIMIT 1 OFFSET " + index;
        DataTable dt = EjecutarConsulta(sql);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public DataRow ObtenerDesafioPorIndexUnidad3(int index)
    {
        string sql = "SELECT * FROM desafios3 LIMIT 1 OFFSET " + index;
        DataTable dt = EjecutarConsulta(sql);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }
}
