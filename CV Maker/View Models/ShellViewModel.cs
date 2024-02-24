using ControlzEx.Theming;
using CV_Maker.DTOs;
using CV_Maker.Interfaces;
using CV_Maker.Models;
using CV_Maker.Templates;
using GalaSoft.MvvmLight.Command;
using ISO3166;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Xps.Packaging;

namespace CV_Maker.View_Models
{
    public class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            OpenFiles = new ObservableCollection<FileTabDto>();

            Themes = ThemeManager.Current.Themes.OrderByDescending(x => x.BaseColorScheme).ToList();
            SelectedTheme = ThemeManager.Current.DetectTheme(App.Current);

            Countries = Country.List.ToList();
            Templates = new List<Type> { typeof(BasicCVTemplate), typeof(PrettyCVTemplate) };
            SelectedTemplate = typeof(BasicCVTemplate);
        }

        private int _newFileCount = 0;
        private FileTabDto? _selectedFileTab;
        private ObservableCollection<FileTabDto> _openFiles;
        private List<Theme> _themes;
        private Theme _selectedTheme;
        private List<Country> _countries;
        private bool _hasOpenDocuments = false;

        public bool HasOpenDocuments
        {
            get => _hasOpenDocuments;
            set => Set(ref _hasOpenDocuments, value, "HasOpenDocuments");
        }

        public List<Country> Countries
        {
            get => _countries;
            set => Set(ref _countries, value, "Countries");
        }

        private IDocumentPaginatorSource _documentSource;
        public IDocumentPaginatorSource DocumentSource
        {
            get => _documentSource;
            set => Set(ref _documentSource, value, "DocumentSource");
        }

        public ICommand NewFileCommand => new RelayCommand(NewFile);
        public ICommand LoadFileCommand => new RelayCommand(async () => await LoadFile());
        public ICommand SaveFileCommand => new RelayCommand(async () => await SaveFile());
        public ICommand SaveFileAsCommand => new RelayCommand(SaveFileAs);
        public ICommand AddJobCommand => new RelayCommand(AddJob);
        public ICommand AddEducationCommand => new RelayCommand(AddEducation);
        public ICommand RemoveJobCommand => new RelayCommand<Job>(RemoveJob);
        public ICommand RemoveEducationCommand => new RelayCommand<Education>(RemoveEducation);
        public ICommand AddRoleCommand=> new RelayCommand<Job>(AddRole);
        public ICommand RemoveLinkCommand => new RelayCommand<Link>(RemoveLink);
        public ICommand RemoveSkillCommand => new RelayCommand<Skill>(RemoveSkill);
        public ICommand AddLinkCommand => new RelayCommand(AddLink);
        public ICommand AddSkillCommand => new RelayCommand(AddSkill);
        public ICommand ChangeThemeCommand => new RelayCommand<Theme>(ChangeTheme);
        public ICommand CloseTabCommand => new RelayCommand<FileTabDto>(CloseTab);
        public ICommand RemoveRoleCommand => new RelayCommand<Role>(RemoveRole);
        public ICommand TabSelectedCommand => new RelayCommand(async () => await TabSelected());

        public async Task TabSelected()
        {
            //Set loading state here 
            Preview();
        }

        public void AddJob()
        {
            var newJob = new Job { FromDate = DateTime.Now, Roles = new ObservableCollection<Role>() };
            SelectedFileTab?.CV.Jobs.Add(newJob);
        }

        public void RemoveRole(Role role)
        {
            var job = SelectedFileTab?.CV.Jobs.Where(x => x.Roles.Any(x => x == role)).SingleOrDefault();
            job.Roles.Remove(role);
        }

        public void AddLink()
        {
            var newLink = new Link();
            SelectedFileTab?.CV.Links.Add(newLink);
        }
        public void RemoveLink(Link link)
        {
            SelectedFileTab?.CV.Links.Remove(link);
        }

        public void AddSkill()
        {
            var newSkill = new Skill();
            SelectedFileTab?.CV.Skills.Add(newSkill);
        }
        public void RemoveSkill(Skill skill)
        {
            SelectedFileTab?.CV.Skills.Remove(skill);
        }

        public void AddEducation()
        {
            var newEducation = new Education { FromDate = DateTime.Now };
            SelectedFileTab?.CV.Education.Add(newEducation);
        }

        public void RemoveEducation(Education education)
        {
            SelectedFileTab?.CV.Education.Remove(education);
        }

        public void RemoveJob(Job job)
        {
            SelectedFileTab?.CV.Jobs.Remove(job);
        }

        public void CloseTab(FileTabDto tab)
        {
            OpenFiles.Remove(tab);
        }

        public void AddRole(Job job)
        {
            if(job.Roles != null)
            {
                var role = new Role();
                job.Roles.Add(role);
            }
        }

        public void NewFile()
        {
            _newFileCount++;

            var newCv = new CV {
                Jobs = new ObservableCollection<Job>(),
                Links = new ObservableCollection<Link>(),
                Education = new ObservableCollection<Education>(),
                Skills = new ObservableCollection<Skill>(),
                Address = new Address { CountryNumericCode = "826" }, 
                DateOfBirth = DateTime.Now,
            };

            var file = new FileTabDto($"untitled({_newFileCount}).json", newCv);
            OpenFiles.Add(file);

            SelectedFileTab = file;
        }

        public void ChangeTheme(Theme theme)
        {
            ThemeManager.Current.ChangeTheme(App.Current, theme);
        }

        public ICommand PreviewCommand => new RelayCommand(Preview);
        public ICommand ExportPdfCommand => new RelayCommand(ExportPdf);

        private List<Type> _templates;

        public List<Type> Templates
        {
            get => _templates;
            set => Set(ref _templates, value, "Templates");
        }

        private Type _selectedTemplate;

        public Type SelectedTemplate
        {
            get => _selectedTemplate;
            set => Set(ref _selectedTemplate, value, "SelectedTemplate");
        }

        public void Preview()
        {
            if (SelectedFileTab?.CV != null)
            {
                //var template = new BasicCVTemplate(SelectedFileTab?.CV);
                var template = Activator.CreateInstance(SelectedTemplate, SelectedFileTab?.CV) as ICVTemplate;

                if (template != null)
                {
                    template.GenerateXps("report.xps");
                    //var xps = new XpsDocument("report.xps", FileAccess.ReadWrite);
                    using (var xps2 = new XpsDocument("report.xps", FileAccess.Read))
                    {
                        DocumentSource = xps2.GetFixedDocumentSequence();
                    }
                }
            }
        }

        public async Task SaveFile()
        {

        }

        public void ExportPdf()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = SelectedFileTab?.FileName;
            dialog.DefaultExt = ".pdf";
            dialog.Filter = "PDF | *.pdf";

            if (dialog.ShowDialog() == true)
            {
                var template = new BasicCVTemplate(SelectedFileTab?.CV);
                template.GeneratePdf(dialog.FileName);
            }
        }

        public async Task LoadFile()
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = string.Empty;
            dialog.Filter = "Json | *.json";

            if (dialog.ShowDialog() == true)
            {
                _newFileCount++;
                using (var sR = new StreamReader(dialog.FileName))
                {
                    var json = await sR.ReadToEndAsync();

                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new JsonObservableCollectionConverter(),
                    };

                    var cv = JsonConvert.DeserializeObject<CV>(json, settings);

                    var file = new FileTabDto(dialog.SafeFileName, cv);
                    OpenFiles.Add(file);
                    SelectedFileTab = file;
                    //Preview();
                }
            }
        }

        public ICommand PrintCommand => new RelayCommand<DocumentViewer>(Print);

        public void Print(DocumentViewer viewer)
        {
            viewer.Print();
        }

        public void SaveFileAs()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = SelectedFileTab?.FileName;
            dialog.Filter = "Json | *.json";

            if (dialog.ShowDialog() == true)
            {
                var serializer = new JsonSerializer();

                using (StreamWriter sw = new StreamWriter(dialog.FileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, SelectedFileTab?.CV);
                }
            }
        }

        public ObservableCollection<FileTabDto> OpenFiles
        {
            get => _openFiles;
            set => Set(ref _openFiles, value, "OpenFiles");
        }

        public FileTabDto? SelectedFileTab
        {
            get => _selectedFileTab;
            set { Set(ref _selectedFileTab, value, "SelectedFileTab"); HasOpenDocuments = OpenFiles.Any(); }
        }

        public List<Theme> Themes
        {
            get => _themes;
            set => Set(ref _themes, value, "Themes");
        }

        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set => Set(ref _selectedTheme, value, "SelectedTheme");
        }

        public class JsonObservableCollectionConverter : DefaultContractResolver
        {
            public JsonObservableCollectionConverter() : base()
            {

            }

            public override JsonContract ResolveContract(Type type)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    return ResolveContract(typeof(ObservableCollection<>).MakeGenericType(type.GetGenericArguments()));
                }
                

                return base.ResolveContract(type);
            }
        }
    }
}
