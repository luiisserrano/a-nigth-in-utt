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
            Debug.Log("Ruta de la Base de la Base de Datos: " + dbName);
            
            // Inicializar DB aquí para asegurar que exista antes de que otros scripts (como player.Start) la usen
            CreateTables();
            InsertarDatosIniciales();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Log de confirmación solamente, la inicialización real ya ocurrió en Awake
        Debug.Log("Sqlite listo.");
    }

    // --- CREACIÓN DE TABLAS ---
    private void CreateTables()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Tabla Desafios
                command.CommandText = "CREATE TABLE IF NOT EXISTS desafios (id INTEGER PRIMARY KEY AUTOINCREMENT, Desafio TEXT NOT NULL, respuesta TEXT NOT NULL, Unidad INTEGER NOT NULL);";
                command.ExecuteNonQuery();

                // Tabla Items
                command.CommandText = "CREATE TABLE IF NOT EXISTS items (id INTEGER PRIMARY KEY AUTOINCREMENT, item TEXT NOT NULL, activo INTEGER NOT NULL);";
                command.ExecuteNonQuery();

                // Tabla Alumnos
                // Tabla Alumnos
                command.CommandText = "CREATE TABLE IF NOT EXISTS alumnos (id INTEGER PRIMARY KEY AUTOINCREMENT, tag TEXT NOT NULL, visitado INTEGER NOT NULL);";
                command.ExecuteNonQuery();

                // Tabla Maestros (Jefes)
                // Vencido: 0 = No, 1 = Si
                // Tabla Maestros (Jefes)
                // Vencido: 0 = No, 1 = Si
                // ItemsReq: IDs de items (1-12) separados por comas
                command.CommandText = "CREATE TABLE IF NOT EXISTS maestros (id INTEGER PRIMARY KEY AUTOINCREMENT, nombre TEXT NOT NULL, vencido INTEGER NOT NULL, items_req TEXT NOT NULL);";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        Debug.Log("Tablas verificadas correctamente.");
    }

    // --- INSERCIÓN DE DATOS INICIALES ---
    private void InsertarDatosIniciales()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            // -----------------------
            // Desafios (Unificada)
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
                            // Unidad 1
                            command.CommandText = "INSERT INTO desafios (Desafio, Respuesta, Unidad) VALUES ('P1U1','R1',1),('P2U1','R2',1),('P3U1','R1',1),('P4U1','R3',1),('P5U1','R2',1),('P6U1','R2',1),('P7U1','R3',1),('P8U1','R1',1),('P9U1','R2',1),('P10U1','R3',1),('P11U1','R2',1),('P12U1','R3',1),('P13U1','R2',1),('P14U1','R1',1);";
                            command.ExecuteNonQuery();

                            // Unidad 2
                            command.CommandText = "INSERT INTO desafios (Desafio, Respuesta, Unidad) VALUES ('P1U2','R1',2),('P2U2','R1',2),('P3U2','R3',2),('P4U2','R3',2),('P5U2','R1',2),('P6U2','R2',2),('P7U2','R1',2),('P8U2','R3',2),('P9U2','R2',2),('P10U2','R3',2),('P11U2','R3',2),('P12U2','R2',2),('P13U2','R1',2),('P14U2','R3',2);";
                            command.ExecuteNonQuery();

                            // Unidad 3
                            command.CommandText = "INSERT INTO desafios (Desafio, Respuesta, Unidad) VALUES ('P1U3','R1',3),('P2U3','R3',3),('P3U3','R2',3),('P4U3','R2',3),('P5U3','R1',3),('P6U3','R1',3),('P7U3','R3',3),('P8U3','R1',3),('P9U3','R2',3),('P10U3','R3',3),('P11U3','R2',3),('P12U3','R1',3),('P13U3','R2',3),('P14U3','R1',3);";
                            command.ExecuteNonQuery();

                            transaction.Commit();
                        }
                    }
                    Debug.Log("Tabla 'desafios' poblada con datos de la Unidad 1, 2 y 3.");
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

            // -----------------------
            // Alumnos
            // -----------------------
            using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM alumnos";
                if ((long)checkCmd.ExecuteScalar() == 0)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                                INSERT INTO alumnos (tag, visitado) VALUES
                                ('julie',0),('charly',0),('npc1',0),('jared',0),('noc',0),
                                ('fat',0),('luis',0),('choche',0),('mark',0),('punk',0),
                                ('aze',0),('vict',0),('p1',0);";
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    Debug.Log("Tabla 'alumnos' creada y poblada correctamente.");
                }
            }

            // -----------------------
            // Maestros
            // -----------------------
            using (var checkCmd = connection.CreateCommand())
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM maestros";
                if ((long)checkCmd.ExecuteScalar() == 0)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            // DelToro (1,2,3), Ramiro (4,5,6), Rosales (7,8,9), Igmar (Todos 1-12)
                            command.CommandText = @"
                                INSERT INTO maestros (nombre, vencido, items_req) VALUES
                                ('delToro',0,'1,2,3'),
                                ('ramiro',0,'4,5,6'),
                                ('rosales',0,'7,8,9'),
                                ('igmar',0,'1,2,3,4,5,6,7,8,9,10,11,12');";
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    Debug.Log("Tabla 'maestros' poblada con requisitos de items.");
                }
            }

            connection.Close();
        }
    }

    // ... (Helpers) ...

    // --- METODOS MAESTROS ---

    public int[] GetMasterRequiredItems(string nombre)
    {
        string sql = "SELECT items_req FROM maestros WHERE nombre = '" + nombre + "'";
        DataTable dt = EjecutarConsulta(sql);
        if (dt.Rows.Count > 0)
        {
            string reqStr = dt.Rows[0]["items_req"].ToString();
            string[] parts = reqStr.Split(',');
            System.Collections.Generic.List<int> result = new System.Collections.Generic.List<int>();
            
            foreach (string p in parts)
            {
                if (int.TryParse(p, out int id))
                {
                    // Convertir ID de BD (1-12) a Índice Array (0-11)
                    result.Add(id - 1);
                }
            }
            return result.ToArray();
        }
        return new int[0]; // Retornar vacío si no encuentra
    }

    public bool IsMasterDefeated(string nombre)
    {
        string sql = "SELECT vencido FROM maestros WHERE nombre = '" + nombre + "'";
        DataTable dt = EjecutarConsulta(sql);
        if (dt.Rows.Count > 0)
        {
            return System.Convert.ToInt32(dt.Rows[0]["vencido"]) == 1;
        }
        return false;
    }

    public void SetMasterDefeated(string nombre)
    {
        string sql = "UPDATE maestros SET vencido = 1 WHERE nombre = '" + nombre + "'";
        ExecuteNonQuery(sql); 
    }

    public bool AreAllMastersDefeated()
    {
        // Maestros requeridos para el desafio final: delToro, ramiro, rosales
        // Igmar es el final, asi que comprobamos los otros 3
        string sql = "SELECT COUNT(*) FROM maestros WHERE vencido = 0 AND nombre IN ('delToro', 'ramiro', 'rosales')";
        DataTable dt = EjecutarConsulta(sql);
        if (dt.Rows.Count > 0)
        {
            long n = (long)dt.Rows[0][0];
            return n == 0;
        }
        return false;
    }
    
    public void ResetMasters()
    {
        string sql = "UPDATE maestros SET vencido = 0";
        ExecuteNonQuery(sql);
        Debug.Log("Maestros reseteados a 0.");
    }
    
    // --- METODOS ALUMNOS ---

    // --- HELPER PARA EJECUTAR NON-QUERY (INSERT, UPDATE, DELETE) ---
    public void ExecuteNonQuery(string sql)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    // --- METODOS ALUMNOS ---

    public bool IsStudentVisited(string tag)
    {
        string sql = "SELECT visitado FROM alumnos WHERE tag = '" + tag + "'";
        DataTable dt = EjecutarConsulta(sql);
        if (dt.Rows.Count > 0)
        {
            return System.Convert.ToInt32(dt.Rows[0]["visitado"]) == 1;
        }
        return false;
    }

    public void SetStudentVisited(string tag)
    {
        string sql = "UPDATE alumnos SET visitado = 1 WHERE tag = '" + tag + "'";
        ExecuteNonQuery(sql); // Uso limpio y único
    }

    public void SetAllStudentsVisited()
    {
        string sql = "UPDATE alumnos SET visitado = 1";
        ExecuteNonQuery(sql);
    }

    public void ResetStudents()
    {
        string sql = "UPDATE alumnos SET visitado = 0";
        ExecuteNonQuery(sql);
        Debug.Log("Alumnos reseteados a 0 en BDD.");
    }

    // --- MÉTODOS GENERALES ---
    // (EjecutarConsulta se mantiene igual para SELECTs)
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

    // --- MÉTODOS DESAFIOS (Unificados) ---
    // Se busca por nombre del desafio (tags P#U#) o offset relativo a la unidad
    public DataRow ObtenerDesafioPorCodigo(string codigoDesafio, int unidad)
    {
        // Ejemplo: codigoDesafio = "P1U1", unidad = 1
        string sql = "SELECT * FROM desafios WHERE Desafio = '" + codigoDesafio + "' AND Unidad = " + unidad;
        DataTable dt = EjecutarConsulta(sql);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    public DataRow ObtenerRespuestaPorIndiceYUnidad(int indexItem, int unidad) 
    {
        // Asumiendo que indexItem va de 0 a N-1 dentro de la unidad
        string sql = "SELECT * FROM desafios WHERE Unidad = " + unidad + " LIMIT 1 OFFSET " + indexItem;
        DataTable dt = EjecutarConsulta(sql);
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    // --- MÉTODOS ITEMS ---

    public void UpdateItemActive(int idItem, bool activo)
    {
        int estado = activo ? 1 : 0;
        string sql = "UPDATE items SET activo = " + estado + " WHERE id = " + idItem;
        ExecuteNonQuery(sql);
    }

    public bool AreAllItemsCollected()
    {
        string sql = "SELECT COUNT(*) FROM items WHERE activo = 0";
        DataTable dt = EjecutarConsulta(sql);
        if (dt.Rows.Count > 0)
        {
            long inactivos = (long)dt.Rows[0][0];
            return inactivos == 0;
        }
        return false;
    }

    public void ResetItems()
    {
        string sql = "UPDATE items SET activo = 0";
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
        Debug.Log("Items reseteados a 0 en BDD.");
    }

    public System.Collections.Generic.List<int> GetActiveItemIndices()
    {
        System.Collections.Generic.List<int> indices = new System.Collections.Generic.List<int>();
        string sql = "SELECT id FROM items WHERE activo = 1";
        DataTable dt = EjecutarConsulta(sql);
        foreach (DataRow row in dt.Rows)
        {
            // ID en BD inicia en 1, así que restamos 1 para ser 0-based
            int id = System.Convert.ToInt32(row["id"]);
            indices.Add(id - 1);
        }
        return indices;
    }
}
