using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SlackWebAPI
{
    public class SlackClientAPI
    {

        private readonly string _token;
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();

        public SlackClientAPI(string token)
        {
            _uri = new Uri("https://slack.com/api/");
            _token = token;
        }

        internal string ArgumentsToJson(Arguments slackMessage)
        {
            byte[] json;
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Arguments));
                serializer.WriteObject(ms, slackMessage);
                ms.Position = 0;
                json = ms.ToArray();
            }
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public Response PostMessage(string APImethod, Arguments args)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                client.Headers[HttpRequestHeader.Authorization] = "Bearer " + _token;
                string jsonString = ArgumentsToJson(args);
                var response = client.UploadData(_uri + APImethod, "POST", _encoding.GetBytes(jsonString));
                string responseText = _encoding.GetString(response);
                return ReadToObject(responseText);
            }
        }

        internal Response ReadToObject(string json)
        {
            Response deserializedResponse = new Response();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedResponse.GetType());
            deserializedResponse = ser.ReadObject(ms) as Response;
            ms.Close();
            return deserializedResponse;
        }

        [DataContract]
        public class Arguments
        {
            internal Arguments()
            {
                Attachments = new ObservableCollection<Attachment>();
            }
            [DataMember(Name = "channel", EmitDefaultValue = false)]
            internal string Channel;
            [DataMember(Name = "name", EmitDefaultValue = false)]
            internal string Name;
            [DataMember(Name = "username", EmitDefaultValue = false)]
            internal string Username;
            [DataMember(Name = "text", EmitDefaultValue = false)]
            internal string Text;
            [DataMember(Name = "timestamp", EmitDefaultValue = false)]
            internal string Timestamp;
            [DataMember(Name = "parse", EmitDefaultValue = false)]
            internal string Parse;
            [DataMember(Name = "link_names", EmitDefaultValue = false)]
            internal string LinkNames;
            [DataMember(Name = "unfurl_links", EmitDefaultValue = false)]
            internal string UnfurlLinks;
            [DataMember(Name = "unfurl_media", EmitDefaultValue = false)]
            internal string UnfurlMedia;
            [DataMember(Name = "icon_url", EmitDefaultValue = false)]
            internal string IconUrl;
            [DataMember(Name = "icon_emoji", EmitDefaultValue = false)]
            internal string IconEmoji;
            [DataMember(Name = "attachments", EmitDefaultValue = false)]
            internal ObservableCollection<Attachment> Attachments;
        }

        [DataContract]
        public class Attachment
        {
            internal Attachment()
            {
                Fields = new ObservableCollection<AttachmentFields>();
            }
            [DataMember(Name = "fallback", EmitDefaultValue = false)]
            internal string Fallback;
            [DataMember(Name = "pretext", EmitDefaultValue = false)]
            internal string Pretext;
            [DataMember(Name = "color", EmitDefaultValue = false)]
            internal string Color;
            [DataMember(Name = "author_name", EmitDefaultValue = false)]
            internal string AuthorName;
            [DataMember(Name = "author_link", EmitDefaultValue = false)]
            internal string AuthorLink;
            [DataMember(Name = "author_icon", EmitDefaultValue = false)]
            internal string AuthorIcon;
            [DataMember(Name = "title", EmitDefaultValue = false)]
            internal string Title;
            [DataMember(Name = "title_link", EmitDefaultValue = false)]
            internal string TitleLink;
            [DataMember(Name = "text", EmitDefaultValue = false)]
            internal string Text;
            [DataMember(Name = "ts", EmitDefaultValue = false)]
            internal long TS;
            [DataMember(Name = "image_url", EmitDefaultValue = false)]
            internal string ImageUrl;
            [DataMember(Name = "footer", EmitDefaultValue = false)]
            internal string Footer;
            [DataMember(Name = "footer_icon", EmitDefaultValue = false)]
            internal string FooterIcon;
            [DataMember(Name = "fields", EmitDefaultValue = false)]
            internal ObservableCollection<AttachmentFields> Fields;
        }

        [DataContract]
        public class AttachmentFields
        {
            [DataMember(Name = "title", EmitDefaultValue = false)]
            internal string Title;
            [DataMember(Name = "value", EmitDefaultValue = false)]
            internal string Value;
            [DataMember(Name = "short", EmitDefaultValue = false)]
            internal bool Short;
        }

        [DataContract]
        public class Response
        {
            [DataMember(Name = "ok", EmitDefaultValue = false)]
            internal bool Ok;
            [DataMember(Name = "channels", EmitDefaultValue = false)]
            internal string Channels;
            [DataMember(Name = "ts", EmitDefaultValue = false)]
            internal string TimeStamp;
            [DataMember(Name = "error", EmitDefaultValue = false)]
            internal string Error;
            [DataMember(Name = "warning", EmitDefaultValue = false)]
            internal string Warning;
        }
    }
}
