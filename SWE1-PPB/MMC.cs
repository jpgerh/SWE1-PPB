using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SWE1_PPB
{
    class MMC
    {
        private string _name = "", _url = "", _filetype = "", _title = "", _artist = "", _album = "", _genre = "", _path = "", _length = "";
        private int _filesize = 0, _rating = 0;
        DBHandler dBHandler = new DBHandler();

        public MMC ()
        {
            _name = "";
            _url = "";
            _filetype = "";
            _title = "";
            _artist = "";
            _album = "";
            _genre = "";
            _path = "";
            _filesize = 0;
            _rating = 0;
            _length = "";
        }

        public MMC (string name, string url, string filetype, string title, string artist, string album, string genre, string path, int filesize, int rating, string length)
        {
            _name = name;
            _url = url;
            _filetype = filetype;
            _title = title;
            _artist = artist;
            _album = album;
            _genre = genre;
            _path = path;
            _filesize = filesize;
            _rating = rating;
            _length = length;
        }

        public string Name { get { return _name; } set { this._name = value; } }
        public string Url { get { return _url; } set { this._name = value; } }
        public string Filetype { get { return _filetype; } set { this._filetype = value; } }
        public string Title { get { return _title; } set { this._title = value; } }
        public string Artist { get { return _artist; } set { this._artist = value; } }
        public string Album { get { return _album; } set { this._album = value; } }
        public string Genre { get { return _genre; } set { this._genre = value; } }
        public string Path { get { return _path; } set { this._path = value; } }
        public int Filesize { get { return _filesize; } set { this._filesize = value; } }
        public int Rating { get { return _rating; } set { this._rating = value; } }
        public string Length { get { return _length; } set { this._length = value; } }

        public async Task<string> AddMMC(string token)
        {
            string select = $"SELECT username FROM users WHERE token = '{token}'";
            string username = await dBHandler.ExecuteSingleSelectSQL(select);

            string insert = $"INSERT INTO library (username, name, url, filetype, title, artist, album, genre, path, filesize, rating, length) VALUES " +
                $"('{username}', '{Name}', '{Url}', '{Filetype}', '{Title}', '{Artist}', '{Album}', '{Genre}', '{Path}', '{Filesize}', '{Rating}', '{Length}')";

            await dBHandler.ExecuteInsertOrDeleteSQL(insert);

            return "Song added successfully.\n";
        }

        public async Task<string> DeleteMMC(string token, string songname)
        {
            string select = $"SELECT username FROM users WHERE token = '{token}'";
            string username = await dBHandler.ExecuteSingleSelectSQL(select);

            string countSelect = $"SELECT COUNT(*) FROM library WHERE username = '{username}' AND name = '{songname}'";
            int count = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(countSelect));

            if (count >= 1)
            {
                string delete = $"DELETE FROM library WHERE username = '{username}' AND name = '{songname}'";
                await dBHandler.ExecuteInsertOrDeleteSQL(delete);
                return "Song deleted successfully.\n";
            } 
            return "Song does not exist in library.\n";

        }

        public async Task<string> AddSongToPlaylist(string token, string songname)
        {
            string selectUsername = $"SELECT username FROM users WHERE token = '{token}'";
            string username = await dBHandler.ExecuteSingleSelectSQL(selectUsername);

            string selectLibraryCount = $"SELECT COUNT(*) FROM library WHERE username = '{username}' AND name = '{songname}'";
            int libraryCount = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectLibraryCount));

            if (libraryCount >= 1)
            {
                string selectPlaylistCount = $"SELECT COUNT(*) FROM playlist";
                int orderNumber = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectPlaylistCount)) + 1;

                string insert = $"INSERT INTO playlist (playlist_order, username, name, url, filetype, title, artist, album, genre, path, filesize, rating, length) VALUES ('{orderNumber}', " +
                    $"(SELECT username FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT name FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT url FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT filetype FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT title FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT artist FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT album FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT genre FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT path FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT filesize FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT rating FROM library WHERE username = '{username}' AND name = '{songname}'), " +
                    $"(SELECT length FROM library WHERE username = '{username}' AND name = '{songname}'))";
                await dBHandler.ExecuteInsertOrDeleteSQL(insert);
                
            } else
            {
                return "Song does not exist in library.\n";
            }
            return "Song added to playlist.\n";

        }

        public async Task<string> getPlaylist()
        {
            DataTable dataTable = new DataTable();

            string sql = $"SELECT * FROM playlist order by playlist_order asc";
            dataTable = await dBHandler.ExecuteSQLGetDT(sql);

            string returnString = "";

            if (dataTable.Rows.Count == 0)
            {
                returnString = "Playlist is empty.\n";
            } 
            else
            {
                returnString = "Playlist:";

                foreach (DataRow row in dataTable.Rows)
                {
                    returnString = returnString + "\n-----------------\nOrder: " + row["playlist_order"] + "\nAdded by: " + row["username"] + "\nSongname: " + row["name"] + "\nUrl: " + row["url"] + "\nFiletype: " + row["filetype"] +
                        "\nTitle: " + row["title"] + "\nArtist: " + row["artist"] + "\nAlbum: " + row["album"] + "\nGenre: " + row["genre"] + "\nPath: " + row["path"] + "\nFilesize: " + row["filesize"].ToString() + "\nRating: " + row["rating"].ToString() + "\nLength: " + row["length"];
                }
            }

            return returnString;
        }

        public async void reorderPlaylist(int fromPosition, int toPosition)
        {

            if (fromPosition > toPosition)
            {

                string update = $"UPDATE playlist SET playlist_order = (playlist_order + 1) WHERE playlist_order >= {toPosition} AND playlist_order < {fromPosition}";
                await dBHandler.ExecuteInsertOrDeleteSQL(update);
            } 
            else if (fromPosition < toPosition)
            {

                string update = $"UPDATE playlist SET playlist_order = (playlist_order - 1) WHERE playlist_order <= {toPosition} AND playlist_order > {fromPosition}";
                await dBHandler.ExecuteInsertOrDeleteSQL(update);
            }

            string update2 = $"UPDATE playlist SET playlist_order = {toPosition} WHERE playlist_order = {fromPosition}";
            await dBHandler.ExecuteInsertOrDeleteSQL(update2);
        }

        public async Task<string> getLibrary(string token)
        {
            DataTable dataTable = new DataTable();

            string select = $"SELECT username FROM users WHERE token = '{token}'";
            string username = await dBHandler.ExecuteSingleSelectSQL(select);

            string sql = $"SELECT * FROM library WHERE username = '{username}'";
            dataTable = await dBHandler.ExecuteSQLGetDT(sql);

            string returnString = "";

            if (dataTable.Rows.Count == 0)
            {
                returnString = "Library is empty.\n";
            }
            else
            {
                returnString = "Library:";

                foreach (DataRow row in dataTable.Rows)
                {
                    returnString = returnString + "\n-----------------\nSongname: " + row["name"] + "\nUrl: " + row["url"] + "\nFiletype: " + row["filetype"] +
                        "\nTitle: " + row["title"] + "\nArtist: " + row["artist"] + "\nAlbum: " + row["album"] + "\nGenre: " + row["genre"] + "\nPath: " + row["path"] + "\nFilesize: " + row["filesize"].ToString() + "\nRating: " + row["rating"].ToString() + "\nLength: " + row["length"];
                }
            }

            return returnString;
        }
    }
}
