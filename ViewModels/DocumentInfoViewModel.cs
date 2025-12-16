using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FooEditEngine;
using FooEditEngine.WinUI;
using FooEditor.WinUI.Models;
using UI = Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using CommunityToolkit.Mvvm.Messaging;

namespace FooEditor.WinUI.ViewModels
{
    public class DocumentInfoViewModel : ViewModelBase, IXmlSerializable
    {

        public FooEditEngine.TextPoint CaretPostion
        {
            get
            {
                return this.DocumentModel.Document.CaretPostion;
            }
        }

        DocumentModel _model;
        public DocumentModel DocumentModel
        {
            get
            {
                return this._model;
            }
            set
            {
                this._model = value;
                this.OnPropertyChanged();
            }
        }

        public AppSettings Settings
        {
            get
            {
                return AppSettings.Current;
            }
        }

        public DocumentInfoViewModel(bool useFileMapping = false)
        {
            this._model = new DocumentModel(useFileMapping);
        }

        public DocumentInfoViewModel(string title, bool useFileMapping = false) : this(useFileMapping)
        {
            this.DocumentModel.Title = title;
        }

        public void ApplyCurrentSetting()
        {
            this._model.ApplyCurrentSetting();
        }

        public void OnActivate()
        {
            this.OnPropertyChanged("Encode");
            this.OnPropertyChanged("LineFeed");
        }

        public DocumentSource CreatePrintDocument()
        {
            var source = new DocumentSource(this.DocumentModel.Document, new FooEditEngine.Padding(20, 20, 20, 20), AppSettings.Current.FontFamily, AppSettings.Current.FontSize);
            source.ParseHF = (s, e) => { return e.Original; };
            source.Header = AppSettings.Current.Header;
            source.Fotter = AppSettings.Current.Footer;
            source.Forground = AppSettings.Current.ForegroundColor;
            source.Keyword1 = AppSettings.Current.KeywordColor;
            source.Keyword2 = AppSettings.Current.KeywordColor;
            source.Literal = AppSettings.Current.LiteralColor;
            source.Comment = AppSettings.Current.CommentColor;
            source.Url = AppSettings.Current.URLColor;
            source.LineBreak = this.DocumentModel.Document.LineBreak;
            if (source.LineBreak == LineBreakMethod.None)
                source.LineBreak = LineBreakMethod.PageBound;
            source.LineBreakCount = this.DocumentModel.Document.LineBreakCharCount;
            source.ParseHF = (s, e) => {
                PrintInfomation info = new PrintInfomation() { Title = this.DocumentModel.Title, PageNumber = e.PageNumber };
                return EditorHelper.ParseHF(e.Original, info);
            };

            return source;
        }

        public async Task ReloadFileAsync(System.Text.Encoding enc)
        {
            if (this.DocumentModel.CurrentFilePath == null)
            {
                throw new InvalidOperationException("ファイルを読み込んだ状態で実行する必要があります");
            }
            var file = await FileModel.GetFileModel(FileModelBuildType.AbsolutePath, this.DocumentModel.CurrentFilePath);
            this.DocumentModel.Document.Clear();
            await this.DocumentModel.LoadFile(file, enc);
            //エンコードが読み込み後に変わる
            this.OnPropertyChanged("Encode");
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement("DocumentInfoViewModel");
            this.DocumentModel = new DocumentModel();
            this.DocumentModel.ReadXml(reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            this.DocumentModel.WriteXml(writer);
        }
    }
}
