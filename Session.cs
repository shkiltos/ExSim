using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Session
    {
        public static float points;
        public static DateTime sessionDate;
        public static Card card;
        public static int totalCardAmount;
        public static int focusedQuestion = -1;
        public static IDbConnection dbconn = (IDbConnection)new SqliteConnection("URI=file:" + Application.dataPath + "/Plugins/EDB3.s3db");

        public static void SaveSession()
        {
            Session.sessionDate = DateTime.Now;
            
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "INSERT INTO SESSION(sDate,points,card_id) VALUES( \"" + Session.sessionDate.ToString() + "\", " + Session.points + ", " + Session.card.ID + ");";

            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();
            dbcmd.Dispose();
            dbconn.Dispose();
            dbconn.Close();
            dbcmd = null;
        }
    }
    
}
