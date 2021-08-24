// Copyright 2020-2021 Aumoa.lib. All right reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

static class LaunchItems
{
    static string _launchJson = @"Content\LaunchItems.json";
    static List<BuildedElement> _elements;

    public struct Element
    {
        public string Name;
        public string Icon;
        public string AppLink;
    }

    public struct BuildedElement
    {
        public Element Source;
        public Image IconImage;
    }

    public static IReadOnlyList<BuildedElement> GetElements()
    {
        return _elements.AsReadOnly();
    }

    public static void LoadJson()
    {
        try
        {
            if (!File.Exists(_launchJson))
            {
                throw new IOException($"Could not found launch json file({_launchJson}).");
            }

            string jsonString = File.ReadAllText(_launchJson);
            var document = JsonDocument.Parse(jsonString);

            List<Element> elements = new();

            var elementsArray = document.RootElement;
            foreach (var element in elementsArray.EnumerateArray())
            {
                Element rawElement = new();
                foreach (var field in element.EnumerateObject())
                {
                    switch (field.Name)
                    {
                        case "Name":
                            rawElement.Name = field.Value.ToString();
                            break;
                        case "Icon":
                            rawElement.Icon = field.Value.ToString();
                            break;
                        case "AppLink":
                            rawElement.AppLink = field.Value.ToString();
                            break;
                        default:
                            throw new ArgumentException($"The name \"{field.Name}\" is not valid field name.");
                    }
                }
                elements.Add(rawElement);
            }

            List<BuildedElement> builds = new();
            foreach (var element in elements)
            {
                BuildedElement builded = new();
                builded.Source = element;
                builded.IconImage = LoadImageSimple(element.Icon, 200, 200);
                builds.Add(builded);
            }

            _elements = builds;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Fatal error", MessageBoxButton.OK);
            throw;
        }
    }

    private static Image LoadImageSimple(string path, int pixelWidth, int pixelHeight)
    {
        Image image = new Image();
        image.Width = pixelWidth;
        image.Height = pixelHeight;

        // Create source
        BitmapImage imageSource = new BitmapImage();

        // Initialize image source as bitmap.
        imageSource.BeginInit();
        imageSource.UriSource = new Uri(Path.Combine(Directory.GetCurrentDirectory(), "Content", path));
        imageSource.EndInit();

        // Assign image source to image control.
        image.Source = imageSource;
        return image;
    }
}