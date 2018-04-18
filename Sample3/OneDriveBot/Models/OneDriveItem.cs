using System;
using Newtonsoft.Json;

namespace OneDriveBot.Models
{
    public class Rootobject
    {
        public string odatacontext { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        [JsonProperty("@microsoft.graph.downloadUrl")]
        public string microsoftgraphdownloadUrl { get; set; }

        public Createdby createdBy { get; set; }
        public DateTime createdDateTime { get; set; }
        public string cTag { get; set; }
        public string description { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public Lastmodifiedby lastModifiedBy { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public Parentreference parentReference { get; set; }
        public int size { get; set; }
        public string webUrl { get; set; }
        public Audio audio { get; set; }
        public File file { get; set; }
        public Filesysteminfo fileSystemInfo { get; set; }
        public Shared shared { get; set; }
        public Image image { get; set; }
        public Photo photo { get; set; }
        public Folder folder { get; set; }
    }

    public class Createdby
    {
        public Application application { get; set; }
        public Device device { get; set; }
        public User user { get; set; }
        public Onedrivesync oneDriveSync { get; set; }
    }

    public class Application
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }

    public class Device
    {
        public string id { get; set; }
    }

    public class User
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }

    public class Onedrivesync
    {
        public string odatatype { get; set; }
        public string id { get; set; }
    }

    public class Lastmodifiedby
    {
        public User user { get; set; }
        public Application application { get; set; }
        public Device device { get; set; }
        public Onedrivesync oneDriveSync { get; set; }
    }


    public class Parentreference
    {
        public string driveId { get; set; }
        public string driveType { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
    }

    public class Audio
    {
        public string album { get; set; }
        public string albumArtist { get; set; }
        public string artist { get; set; }
        public int bitrate { get; set; }
        public int disc { get; set; }
        public int duration { get; set; }
        public string genre { get; set; }
        public bool hasDrm { get; set; }
        public bool isVariableBitrate { get; set; }
        public string title { get; set; }
        public int track { get; set; }
        public int year { get; set; }
    }

    public class File
    {
        public Hashes hashes { get; set; }
        public string mimeType { get; set; }
    }

    public class Hashes
    {
        public string sha1Hash { get; set; }
    }

    public class Filesysteminfo
    {
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
    }

    public class Shared
    {
        public Owner owner { get; set; }
        public string scope { get; set; }
    }

    public class Owner
    {
        public User user { get; set; }
    }


    public class Image
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Photo
    {
        public DateTime takenDateTime { get; set; }
    }

    public class Folder
    {
        public int childCount { get; set; }
        public View view { get; set; }
    }

    public class View
    {
        public string viewType { get; set; }
        public string sortBy { get; set; }
        public string sortOrder { get; set; }
    }
}