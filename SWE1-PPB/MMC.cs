using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_PPB
{
    class MMC
    {
        private string _name, _url, _filetype, _title, _artist, _album, _genre, _path, _length;
        private int _filesize, _rating;

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
        public string Filetype { get { return _filetype; } set { this._filetype = value; } }
        public string Title { get { return _title; } set { this._title = value; } }
        public string Artist { get { return _artist; } set { this._artist = value; } }
        public string Album { get { return _album; } set { this._album = value; } }
        public string Genre { get { return _genre; } set { this._genre = value; } }
        public string Path { get { return _path; } set { this._path = value; } }
        public int Filesize { get { return _filesize; } set { this._filesize = value; } }
        public int Rating { get { return _rating; } set { this._rating = value; } }
        public string Length { get { return _length; } set { this._length = value; } }

    }
}
