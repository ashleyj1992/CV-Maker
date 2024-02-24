using CV_Maker.Interfaces;
using CV_Maker.Models;
using Microcharts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace CV_Maker.Templates
{
    public class BasicCVTemplate : ICVTemplate
    {
        private CV _cv { get; set; }
        public CV CV { get => _cv; set => _cv = value; }

        private TextStyle _headerStyle = TextStyle.Default.FontSize(20).Black().FontColor(Colors.Black);
        private TextStyle _nameStyle = TextStyle.Default.FontSize(30).Black().FontColor(_textColour);
        private TextStyle _header1Style = TextStyle.Default.FontSize(20).Black().FontColor(Colors.Black);
        private TextStyle _header2Style = TextStyle.Default.FontSize(14).Black().FontColor(Colors.Grey.Darken2);
        private TextStyle _textStyle1Dark = TextStyle.Default.FontSize(8).Black().FontColor(Colors.Black);
        private TextStyle _textStyle1Light = TextStyle.Default.FontSize(8).Black().FontColor(Colors.White);
        public BasicCVTemplate(CV cv)
        {
            _cv = cv;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        private const string _headerBackgroundColour = "#191919";
        private const string _footerBackgroundColour = "#191919";
        private const string _backgroundItemBackgroundColour = "#F8F4EC";
        private const string _jobsItemBackgroundColour = "#F8F4EC";
        private const string _skillsItemBackgroundColour = "#F8F4EC";
        private const string _educationItemBackgroundColour = "#F8F4EC";
        private const string _textColour = "#FFF8E3";
        private const string _basicTextColour = "#111111";


        void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(10).SemiBold().FontColor(_textColour);
            
            container.Background(_headerBackgroundColour).Padding(5).Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item()
                    .Text($"{_cv.FirstName} {_cv.LastName}")
                    .Style(_nameStyle);
                });

                row.RelativeColumn().Column(column =>
                {
                    column.Item()
                    .AlignRight()
                    .PaddingBottom(10)
                    .ContentFromRightToLeft()
                    .Text(_cv.Address.ToString())
                    .Style(titleStyle).ExtraBlack();

                    column.Item()
                    .AlignRight()
                    .PaddingBottom(5)
                    .Text($"Date of birth: {_cv.DateOfBirth.ToString("dd MMM yyyy")}")
                    .Style(titleStyle);

                    column.Item()
                    .AlignRight()
                    .PaddingBottom(5)
                    .Hyperlink($"mailto:{_cv.EmailAddress}")
                    .Text($"Email address: {_cv.EmailAddress}")
                    .Style(titleStyle);

                    column.Item()
                    .AlignRight()
                    .PaddingBottom(5)
                    .Hyperlink($"tel:{_cv.MobileNumber}")
                    .Text($"Mobile number: {_cv.MobileNumber}")
                    .Style(titleStyle);
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Element(x => ComposeBackground(x, _backgroundItemBackgroundColour));
                    column.Spacing(5);
                    column.Item().Element(x => ComposeJobs(x, _jobsItemBackgroundColour));
                    column.Spacing(5);
                    column.Item().Element(x => ComposeSkills(x, _skillsItemBackgroundColour));
                    column.Spacing(5);
                    column.Item().Element(x => ComposeEducation(x, _educationItemBackgroundColour));
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Background(_footerBackgroundColour).Column(column =>
                {
                    foreach (var link in CV.Links)
                    {
                        column.Item()
                        .Text(x =>
                        {
                            x.Span($"{link.Name}: ").Style(_textStyle1Light);
                            x.Hyperlink($"{link.URL}", $"{link.URL}").Style(_textStyle1Light);
                        });
                    }

                    column.Item().BorderColor(Colors.White).BorderBottom(1).PaddingBottom(5);

                    var canDrive = CV.CanDrive ? "can drive" : "can't drive";
                    var ownCar = CV.OwnCar ? "Has own car" : "Doesnt own a car";

                    column.Item().Text($"{ownCar} and {canDrive}").Style(_textStyle1Light);

                    column.Item().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber().Style(_textStyle1Light);
                        x.Span(" / ").Style(_textStyle1Light);
                        x.TotalPages().Style(_textStyle1Light);
                    });
                });
            });
        }

        public void ComposeJobs(IContainer container, string color)
        {
            if(CV.Jobs.Any())
            {
                container.Background(color).Padding(10).Column(column =>
                {
                    column.Spacing(5);
                    column.Item().Text("Jobs").Style(_headerStyle);
                    column.Item().BorderBottom(1).PaddingBottom(5);

                    foreach (var job in CV.Jobs)
                    {
                        var fromToDate = job.ToDate == null ? $"{job.FromDate.ToString("dd MMM yyyy")} - Current" : $"{job.FromDate.ToString("dd MMM yyyy")} - {job.ToDate?.ToString("dd MMM yyyy")}";

                        column.Item().Text(x =>
                        {
                            x.Span(job.CompanyName).Style(_header1Style);
                            x.Span($" - {fromToDate}").Style(_header1Style);
                        });

                        foreach (var role in job.Roles)
                        {
                            column.Item().Text(role.RoleName).Style(_header2Style);
                            column.Item().Padding(5).Text(role.Description).Style(_textStyle1Dark);
                            column.Item().PaddingBottom(5);
                        }
                        column.Item().BorderBottom(1).PaddingBottom(5);
                    }
                });
            }
        }

        public void ComposeSkills(IContainer container, string color)
        {
            if(CV.Skills.Any())
            {
                container.Background(color).Padding(10).Column(column =>
                {
                    column.Spacing(5);
                    column.Item().Text("Skills").Style(_headerStyle);
                    column.Item().BorderBottom(1).PaddingBottom(5);

                    var ent = new List<ChartEntry>();

                    foreach (var skill in CV.Skills)
                    {
                        ent.Add(new ChartEntry(skill.Experience)
                        {
                            Label = skill.SkillName,
                            ValueLabel = skill.Experience.ToString(),
                            Color = SKColor.Parse(_basicTextColour),
                            TextColor = SKColor.Parse(_basicTextColour),
                            ValueLabelColor = SKColor.Parse(_basicTextColour)
                        });
                    }

                    column
                    .Item()
                    .Height(200)
                    .Canvas((canvas, size) =>
                    {
                        var chart = new BarChart
                        {
                            Entries = ent,
                            MaxValue = 100,
                            LabelTextSize = 8f,
                            LabelColor = SKColor.Parse(_basicTextColour),
                            LabelOrientation = Microcharts.Orientation.Horizontal,
                            ValueLabelOrientation = Microcharts.Orientation.Horizontal,
                            IsAnimated = false,
                        };

                        chart.DrawContent(canvas, (int)size.Width, (int)size.Height);
                    });
                });
            }
        }

        public void ComposeEducation(IContainer container, string color)
        {
            container.Background(color).Padding(10).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("Education").Style(_headerStyle);
                column.Item().BorderBottom(1).PaddingBottom(5);
                foreach (var education in CV.Education)
                {
                    var fromToDate = education.ToDate == null ? $"{education.FromDate.ToString("dd MMM yyyy")} - Current" : $"{education.FromDate.ToString("dd MMM yyyy")} - {education.ToDate?.ToString("dd MMM yyyy")}";

                    column.Item().Text(x =>
                    {
                        x.Span(education.EducationName).Style(_header1Style);
                        x.Span($" - {fromToDate}").Style(_header1Style);
                    });

                    column.Item().Padding(5).Text(education.CourseName.ToString()).Style(_header2Style);
                    column.Item().BorderBottom(1).PaddingBottom(5);
                }
            });
        }

        public void ComposeBackground(IContainer container, string color)
        {
            container.Background(color).Padding(10).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("Background").Style(_headerStyle);
                column.Item().BorderBottom(1).PaddingBottom(5);
                column.Item().Padding(5).Text(CV.BackgroundDescription).Style(_textStyle1Dark);
            });
        }

        public void Compose(IDocumentContainer container)
        {
            container
            .Page(page =>
            {
                page.PageColor(_headerBackgroundColour);
                page.Margin(20);

                page.Header().Element(ComposeHeader);

                page.Content().Element(x => ComposeContent(x));

                page.Footer().Element(ComposeFooter);
            });
        }
    }
}
