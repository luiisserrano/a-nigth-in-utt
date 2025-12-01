using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;

public class Sqlite : MonoBehaviour
{
    public static Sqlite instance;
    private string dbName;

    private void Awake()
    {
        // Patrón Singleton para que sobreviva entre escenas
        if (instance == null)
        {
            instance = this;
            // ESTA LÍNEA ES LA MAGIA: Hace que el objeto no se borre al cambiar de escena
            DontDestroyOnLoad(gameObject);

            // Tu código de base de datos
            dbName = "URI=file:" + Path.Combine(Application.persistentDataPath, "DBGame.db");
            Debug.Log("Base de datos ubicada en: " + dbName);
        }
        else
        {
            // Si volvemos al menú y ya existe un Sqlite, destruimos el nuevo para no tener duplicados
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        CreateTables();

        // Lógica de inicio de sesión:
        if (ExisteJugador())
        {
            CargarJugador();
        }
        else
        {
            Debug.Log("No se encontró usuario. Creando uno nuevo...");
            CrearNuevoJugador("Aventurero", 1); // Nombre por defecto y nivel 1
        }
    }
    private bool ExisteJugador()
    {
        bool existe = false;

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Seleccionamos "COUNT" para ver cuántas filas hay. Es más rápido que traer todos los datos.
                command.CommandText = "SELECT COUNT(*) FROM jugador";

                // ExecuteScalar se usa cuando la consulta devuelve UN solo valor (un número, un string)
                long count = (long)command.ExecuteScalar();

                if (count > 0) existe = true;
            }
            connection.Close();
        }
        return existe;
    }

    // FUNCIÓN 2: Insertar el nuevo jugador
    public void CrearNuevoJugador(string nombre, int nivelInicial)
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Usamos PARAMETROS (@nombre, @nivel) para evitar errores y hackeos (SQL Injection)
                command.CommandText = "INSERT INTO jugador (nombre_usuario, nivel, experiencia, fecha_registro) VALUES (@nombre, @nivel, 0, datetime('now', 'localtime'))";

                command.Parameters.Add(new SqliteParameter("@nombre", nombre));
                command.Parameters.Add(new SqliteParameter("@nivel", nivelInicial));

                command.ExecuteNonQuery();
                Debug.Log($"Jugador '{nombre}' creado exitosamente.");
            }
            connection.Close();
        }
    }

    // FUNCIÓN EXTRA: Cargar datos para verificar
    private void CargarJugador()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Traemos el último jugador creado (o el primero, según lógica)
                command.CommandText = "SELECT nombre_usuario, nivel FROM jugador LIMIT 1";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Leemos los datos de las columnas 0 (nombre) y 1 (nivel)
                        string nombre = reader.GetString(0);
                        int nivel = reader.GetInt32(1);
                        Debug.Log($"Bienvenido de vuelta, {nombre}. Nivel: {nivel}");
                    }
                }
            }
            connection.Close();
        }
    }

    private void CreateTables()
    {
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                string sqlcreation = "";

                // -- TUS 10 TABLAS (Tal cual las tenías) --
                sqlcreation += "CREATE TABLE IF NOT EXISTS jugador (id_jugador INTEGER PRIMARY KEY AUTOINCREMENT, nombre_usuario TEXT NOT NULL, nivel INTEGER DEFAULT 1, experiencia INTEGER DEFAULT 0, fecha_registro TEXT); ";
                sqlcreation += "CREATE TABLE IF NOT EXISTS item (id_item INTEGER PRIMARY KEY AUTOINCREMENT, nombre TEXT NOT NULL, descripcion TEXT, tipo TEXT); ";
                sqlcreation += "CREATE TABLE IF NOT EXISTS jefe (id_jefe INTEGER PRIMARY KEY AUTOINCREMENT, nombre TEXT NOT NULL, descripcion TEXT, dificultad TEXT, nivel_requerido INTEGER); ";
                sqlcreation += "CREATE TABLE IF NOT EXISTS jugador_item (id_jugador_item INTEGER PRIMARY KEY AUTOINCREMENT, id_jugador INTEGER, id_item INTEGER, cantidad INTEGER DEFAULT 1, fecha_obtenido TEXT, FOREIGN KEY (id_jugador) REFERENCES jugador(id_jugador), FOREIGN KEY (id_item) REFERENCES item(id_item)); ";
                sqlcreation += "CREATE TABLE IF NOT EXISTS jefe_item_requerido (id_jefe_item INTEGER PRIMARY KEY AUTOINCREMENT, id_jefe INTEGER, id_item INTEGER, cantidad_requerida INTEGER, FOREIGN KEY (id_jefe) REFERENCES jefe(id_jefe), FOREIGN KEY (id_item) REFERENCES item(id_item)); ";
                sqlcreation += "CREATE TABLE IF NOT EXISTS desafio (id_desafio INTEGER PRIMARY KEY AUTOINCREMENT, id_jefe INTEGER, tipo TEXT, enunciado TEXT, dificultad TEXT, FOREIGN KEY (id_jefe) REFERENCES jefe(id_jefe)); ";
                sqlcreation += "CREATE TABLE IF NOT EXISTS jugador_desafio (id_jugador_desafio INTEGER PRIMARY KEY AUTOINCREMENT, id_jugador INTEGER, id_desafio INTEGER, completado INTEGER DEFAULT 0, puntaje_obtenido INTEGER, fecha_realizado TEXT, FOREIGN KEY (id_jugador) REFERENCES jugador(id_jugador), FOREIGN KEY (id_desafio) REFERENCES desafio(id_desafio)); ";
                sqlcreation += "CREATE TABLE IF NOT EXISTS pregunta_desafio (id_pregunta INTEGER PRIMARY KEY AUTOINCREMENT, id_desafio INTEGER, enunciado TEXT, tipo TEXT, FOREIGN KEY (id_desafio) REFERENCES desafio(id_desafio)); ";
                sqlcreation += "CREATE TABLE IF NOT EXISTS opcion_pregunta (id_opcion INTEGER PRIMARY KEY AUTOINCREMENT, id_pregunta INTEGER, texto TEXT, es_correcta INTEGER DEFAULT 0, FOREIGN KEY (id_pregunta) REFERENCES pregunta_desafio(id_pregunta)); ";
                sqlcreation += "CREATE TABLE IF NOT EXISTS jugador_respuesta (id_jugador_respuesta INTEGER PRIMARY KEY AUTOINCREMENT, id_jugador INTEGER, id_pregunta INTEGER, id_opcion INTEGER, es_correcta INTEGER, fecha_respuesta TEXT, FOREIGN KEY (id_jugador) REFERENCES jugador(id_jugador), FOREIGN KEY (id_pregunta) REFERENCES pregunta_desafio(id_pregunta), FOREIGN KEY (id_opcion) REFERENCES opcion_pregunta(id_opcion)); ";

                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
