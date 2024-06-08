using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using FileExplorer.Models.Entities;

namespace FileExplorer.ViewModels;

public class MetadataViewModel : ObservableRecipient
{
    private File _file;

    private string _title;
    private string _creator;
    private string _subject;
    private string _description;
    private string _publisher;
    private string _contributor;
    private DateTime? _date;
    private string _type;
    private string _format;
    private string _identifier;
    private string _source;
    private string _language;
    private string _relation;
    private string _coverage;
    private string _rights;

    public MetadataViewModel()
    {
        
    }
    
    public MetadataViewModel(File file)
    {
        _file = file;

        // Initialize properties with file metadata
        Title = _file.Title ?? string.Empty;
        Creator = _file.Creator ?? string.Empty;
        Subject = _file.Subject ?? string.Empty;
        Description = _file.Description ?? string.Empty;
        Publisher = _file.Publisher ?? string.Empty;
        Contributor = _file.Contributor ?? string.Empty;
        Date = _file.Date ?? DateTime.Now;
        Type = _file.Type ?? string.Empty;
        Format = _file.Format ?? string.Empty;
        Identifier = _file.Identifier ?? string.Empty;
        Source = _file.Source ?? string.Empty;
        Language = _file.Language ?? string.Empty;
        Relation = _file.Relation ?? string.Empty;
        Coverage = _file.Coverage ?? string.Empty;
        Rights = _file.Rights ?? string.Empty;
    }
    
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Creator
    {
        get => _creator;
        set => SetProperty(ref _creator, value);
    }

    public string Subject
    {
        get => _subject;
        set => SetProperty(ref _subject, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string Publisher
    {
        get => _publisher;
        set => SetProperty(ref _publisher, value);
    }

    public string Contributor
    {
        get => _contributor;
        set => SetProperty(ref _contributor, value);
    }

    public DateTime? Date
    {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    public string Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    public string Format
    {
        get => _format;
        set => SetProperty(ref _format, value);
    }

    public string Identifier
    {
        get => _identifier;
        set => SetProperty(ref _identifier, value);
    }

    public string Source
    {
        get => _source;
        set => SetProperty(ref _source, value);
    }

    public string Language
    {
        get => _language;
        set => SetProperty(ref _language, value);
    }

    public string Relation
    {
        get => _relation;
        set => SetProperty(ref _relation, value);
    }

    public string Coverage
    {
        get => _coverage;
        set => SetProperty(ref _coverage, value);
    }

    public string Rights
    {
        get => _rights;
        set => SetProperty(ref _rights, value);
    }

    public File GetFileWithMetadata()
    {
        _file.Title = Title;
        _file.Creator = Creator;
        _file.Subject = Subject;
        _file.Description = Description;
        _file.Publisher = Publisher;
        _file.Contributor = Contributor;
        _file.Date = Date;
        _file.Type = Type;
        _file.Format = Format;
        _file.Identifier = Identifier;
        _file.Source = Source;
        _file.Language = Language;
        _file.Relation = Relation;
        _file.Coverage = Coverage;
        _file.Rights = Rights;

        return _file;
    }
}