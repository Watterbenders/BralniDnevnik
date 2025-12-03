using System;
using System.Linq;

namespace BookTracker.Domain
{
    public interface IThemeService
    {
        ThemeSettings SetThemeMode(Guid userId, ThemeMode mode);              // US29
        ThemeSettings SetCustomStyle(Guid userId, string? primaryColor,
                                     string? backgroundImage, string? font);  // US30
        ThemeSettings ResetToDefault(Guid userId);
        ThemeSettings GetTheme(Guid userId);
    }

    public class ThemeService : IThemeService
    {
        private readonly InMemoryDataStore _db;

        public ThemeService(InMemoryDataStore db)
        {
            _db = db;
        }

        public ThemeSettings SetThemeMode(Guid userId, ThemeMode mode)
        {
            var t = GetOrCreate(userId);
            t.Mode = mode;
            return t;
        }

        public ThemeSettings SetCustomStyle(Guid userId, string? primaryColor,
            string? backgroundImage, string? font)
        {
            var t = GetOrCreate(userId);
            t.PrimaryColor = primaryColor;
            t.BackgroundImage = backgroundImage;
            t.FontFamily = font;
            return t;
        }

        public ThemeSettings ResetToDefault(Guid userId)
        {
            var t = GetOrCreate(userId);
            t.Mode = ThemeMode.Light;
            t.PrimaryColor = null;
            t.BackgroundImage = null;
            t.FontFamily = null;
            return t;
        }

        public ThemeSettings GetTheme(Guid userId) => GetOrCreate(userId);

        private ThemeSettings GetOrCreate(Guid userId)
        {
            var t = _db.Themes.SingleOrDefault(x => x.UserId == userId);
            if (t == null)
            {
                t = new ThemeSettings { UserId = userId, Mode = ThemeMode.Light };
                _db.Themes.Add(t);
            }

            return t;
        }
    }
}
