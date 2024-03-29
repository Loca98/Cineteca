﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace WCFServer
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di classe "ServerServices" nel codice e nel file di configurazione contemporaneamente. 
    public class ServerServices : IServerServices
    {
        MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString);

        public bool DoWork()        //usata solo per controllare connettivita' dal client  
        {
            return true;
        }

        public Utente GetUser(string email) //Restituise Un oggetto utente contentente i dati dell'utente 
        {
            Utente myUtente = new Utente();
            try
            {
                conn.Open();
                string strQuery = "SELECT * FROM UTENTE where email = '" + email + "'";
                MySqlCommand cmd = new MySqlCommand(strQuery, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    myUtente = new Utente(Convert.ToInt32(reader["Id"]), reader["Email"].ToString(), reader["Passw"].ToString(), reader["Nome"].ToString(), reader["Cognome"].ToString(), Convert.ToBoolean(reader["IsAdmin"]));
                }
                conn.Close();
                conn.Dispose();
                return myUtente;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                conn.Close();
                conn.Dispose();
                return myUtente;
            }
        }

        public bool RegisterUser(string email, string passw, string nome, string cognome, int isAdmin) //Registrazione dell'utente su db 
        {
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    MySqlTransaction transaction = conn.BeginTransaction();
                    string cmd_string = "INSERT INTO UTENTE (Email, Passw, Nome, Cognome, IsAdmin) VALUES ('" + email + "', '" + passw + "', '" + nome + "', '" + cognome + "', '" + isAdmin + "')";
                    MySqlCommand cmd = new MySqlCommand(cmd_string, conn, transaction);                 
                    try
                    {
                        transaction.Commit();
                        conn.Close();
                        conn.Dispose();
                        return true;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        transaction.Rollback();
                        conn.Close();
                        conn.Dispose();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Connessione DB fallita\n");
                    conn.Close();
                    conn.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                conn.Dispose();
                return false;
            }
        }

        public bool LoginUser(string email, string passw)//Login dell'utente 
        {
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    //Cerco sul DB se credenziali corrette 
                    string login_string = "SELECT * FROM UTENTE Where Email = '" + email + "' AND Passw = '" + passw + "'";
                    MySqlCommand cmd = new MySqlCommand(login_string, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        conn.Close();
                        conn.Dispose();
                        return true;
                    }
                    else
                    {
                        conn.Close();
                        conn.Dispose();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Connessione DB fallita\n");
                    conn.Close();
                    conn.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                conn.Dispose();
                return false;
            }

        }

        public List<Film> StoreFilmsList() //Ritorna lista di film dal DB da caricare nella home 
        {
            List<Film> films = new List<Film>();

            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string query = "SELECT * FROM FILM";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Film film = new Film(Convert.ToInt32(reader.GetString(0)), reader.GetString(1), reader.GetString(2), Convert.ToBoolean(reader.GetString(3)), reader.GetString(4));
                        films.Add(film);
                    }
                    conn.Close();
                    conn.Dispose();
                    return films;
                }
                else
                {
                    Console.WriteLine("Connessione DB fallita\n");
                    conn.Close();
                    conn.Dispose();
                    return films;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                conn.Dispose();
                return films;
            }
        }

        public bool RentFilm(int user_id, int film_id, string start_nol, string stop_nol)
        { //Restituire film noleggiato 

            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    MySqlTransaction transaction = conn.BeginTransaction();
                    string cmd_string = "INSERT INTO NOLEGGIO (IdUtente, IdFilm, InizioNoleggio, FineNoleggio) VALUES ('" + user_id + "', '" + film_id + "', '" + start_nol + "', '" + stop_nol + "')";
                    MySqlCommand cmd = new MySqlCommand(cmd_string, conn, transaction);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        conn.Close();
                        conn.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        transaction.Rollback();
                        conn.Close();
                        conn.Dispose();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Connessione DB fallita\n");
                    conn.Close();
                    conn.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                conn.Dispose();
                return false;
            }
        }

        public bool SetFilmStatus(int film_id, bool disp)
        { //Si rende indisponibile il film 
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    MySqlTransaction transaction = conn.BeginTransaction();
                    string cmd_string = "UPDATE FILM SET Disponibile = " + Convert.ToInt32(disp) + " WHERE FILM.Id = " + film_id + ";";
                    MySqlCommand cmd = new MySqlCommand(cmd_string, conn, transaction);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        conn.Close();
                        conn.Dispose();
                        return true;                      
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        transaction.Rollback();
                        conn.Close();
                        conn.Dispose();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Connessione DB fallita\n");
                    Console.WriteLine();
                    conn.Close();
                    conn.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                conn.Close();
                conn.Dispose();
                return false;
            }
        }

        public List<Film> LibraryFilmsList(int user_id)
        {

            List<Film> films = new List<Film>();
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string query = "SELECT * FROM NOLEGGIO, FILM WHERE IdFilm = FILM.Id AND IdUtente = " + user_id;
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Film film = new Film(Convert.ToInt32(reader["IdFilm"]), reader["Titolo"].ToString(), reader["Descrizione"].ToString(), Convert.ToBoolean(reader["Disponibile"]), reader["UrlImage"].ToString());
                        films.Add(film);
                    }
                    conn.Close();
                    conn.Dispose();
                    return films;
                }
                else
                {
                    Console.WriteLine("Connessione DB fallita\n");
                    conn.Close();
                    conn.Dispose();
                    return films;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                conn.Dispose();
                return films;
            }
        }

        public bool ReturnFilm(int user_id, int film_id)
        {
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    MySqlTransaction transaction = conn.BeginTransaction();              
                    string cmd_delete = "DELETE FROM NOLEGGIO WHERE IdUtente = " + user_id + " AND IdFilm = " + film_id;
                    MySqlCommand cmd = new MySqlCommand(cmd_delete, conn, transaction);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        conn.Close();
                        conn.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        transaction.Rollback();
                        conn.Close();
                        conn.Dispose();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Connessione DB fallita\n");
                    conn.Close();
                    conn.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                conn.Dispose();
                return false;
            }
        }

        public bool InsertFilm(string titolo, string descrizione, bool disponibile, string url_image) //Inserisce film nel DB 
        { 

            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    MySqlTransaction transaction = conn.BeginTransaction();
                    string cmd_string = "INSERT INTO FILM (Titolo, Descrizione, Disponibile, UrlImage) VALUES ('" + titolo + "', '" + descrizione + "', " + disponibile + ", '" + url_image + "')";
                    MySqlCommand cmd = new MySqlCommand(cmd_string, conn, transaction);

                    try
                    {
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            conn.Close();
                            conn.Dispose();
                            return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        transaction.Rollback();
                        conn.Close();
                        conn.Dispose();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Connessione DB fallita\n");
                    conn.Close();
                    conn.Dispose();
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                conn.Dispose();
                return false;
            }
        }

        public bool RemoveFilm(string titolo)   //Elimina film nel DB 
        {
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    MySqlTransaction transaction = conn.BeginTransaction();
                    string cmd_string = "DELETE FROM FILM WHERE Titolo = '" + titolo + "'";
                    MySqlCommand cmd = new MySqlCommand(cmd_string, conn, transaction);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        conn.Close();
                        conn.Dispose();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        transaction.Rollback();
                        conn.Close();
                        conn.Dispose();
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Connessione DB fallita\n");
                    conn.Close();
                    conn.Dispose();
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                conn.Dispose();
                return false;
            }
        }

        public bool GetFilmDisp(int film_id)
        {
            bool status = false;
            try
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    string cmd_string = "SELECT * FROM FILM WHERE Id = " + film_id;
                    MySqlCommand cmd = new MySqlCommand(cmd_string, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["Disponibile"].ToString() == "True")
                        {
                            status = true;
                        }
                    }
                    conn.Close();
                    return status;
                }
                else
                {
                    return status = false; ;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                conn.Close();
                conn.Dispose();
                return status = false;
            }
        }
    }
}

