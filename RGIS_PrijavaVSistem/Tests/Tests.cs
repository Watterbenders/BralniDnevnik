using Microsoft.VisualStudio.TestTools.UnitTesting;
using BookTracker.Domain;
using System;

namespace BookTracker.Tests {
    [TestClass]
    public class UserTests {
        [TestMethod]
        public void Test_User_HasNewGuid() {
            User u = new User();
            Assert.AreNotEqual(Guid.Empty, u.Id);
        }

        [TestMethod]
        public void Test_User_DefaultIsPublic() {
            User u = new User();
            Assert.IsTrue(u.IsPublic);
        }

        [TestMethod]
        public void Test_User_StoresUsername() {
            User u = new User();
            u.Username = "Ana";

            Assert.AreEqual("Ana", u.Username);
        }
    }


    [TestClass]
    public class BookTests {
        [TestMethod]
        public void Test_Book_DefaultStatus_IsNaCakanju() {
            DomainBook b = new DomainBook();
            Assert.AreEqual(BookStatus.NaCakanju, b.Status);
        }

        [TestMethod]
        public void Test_Book_CanSetAndGetRating() {
            DomainBook b = new DomainBook();
            b.Rating = 5;

            Assert.AreEqual(5, b.Rating);
        }

        [TestMethod]
        public void Test_Book_FinishedAt_CanBeUpdated() {
            DomainBook b = new DomainBook();
            DateTime dt = DateTime.UtcNow;

            b.FinishedAt = dt;

            Assert.AreEqual(dt, b.FinishedAt);
        }
    }

    [TestClass]
    public class BookQuoteTests {
        [TestMethod]
        public void Test_BookQuote_HasNewGuid() {
            BookQuote q = new BookQuote();
            Assert.AreNotEqual(Guid.Empty, q.Id);
        }

        [TestMethod]
        public void Test_BookQuote_DefaultTextEmpty() {
            BookQuote q = new BookQuote();
            Assert.AreEqual(string.Empty, q.Text);
        }

        [TestMethod]
        public void Test_BookQuote_AssignsBookId() {
            Guid id = Guid.NewGuid();
            BookQuote q = new BookQuote { BookId = id };

            Assert.AreEqual(id, q.BookId);
        }
    }

    [TestClass]
    public class ReadingGoalTests {
        [TestMethod]
        public void Test_ReadingGoal_AssignsYear() {
            ReadingGoal g = new ReadingGoal { Year = 2025 };
            Assert.AreEqual(2025, g.Year);
        }

        [TestMethod]
        public void Test_ReadingGoal_TargetBooksStored() {
            ReadingGoal g = new ReadingGoal { TargetBooks = 24 };
            Assert.AreEqual(24, g.TargetBooks);
        }

        [TestMethod]
        public void Test_ReadingGoal_AssignsUserId() {
            Guid id = Guid.NewGuid();
            ReadingGoal g = new ReadingGoal { UserId = id };

            Assert.AreEqual(id, g.UserId);
        }
    }

    [TestClass]
    public class ReadingProgressTests {
        [TestMethod]
        public void Test_ReadingProgress_DefaultPagesReadIsZero() {
            ReadingProgress rp = new ReadingProgress();
            Assert.AreEqual(0, rp.PagesRead);
        }

        [TestMethod]
        public void Test_ReadingProgress_StoresPages() {
            ReadingProgress rp = new ReadingProgress { PagesRead = 80 };
            Assert.AreEqual(80, rp.PagesRead);
        }

        [TestMethod]
        public void Test_ReadingProgress_AssignsBookId() {
            Guid id = Guid.NewGuid();
            ReadingProgress rp = new ReadingProgress { BookId = id };

            Assert.AreEqual(id, rp.BookId);
        }
    }

    [TestClass]
    public class ReminderSettingsTests {
        [TestMethod]
        public void Test_ReminderSettings_AssignsUserId() {
            Guid id = Guid.NewGuid();
            ReminderSettings r = new ReminderSettings { UserId = id };

            Assert.AreEqual(id, r.UserId);
        }

        [TestMethod]
        public void Test_ReminderSettings_TimeOfDayStored() {
            TimeSpan t = new TimeSpan(8, 30, 0);
            ReminderSettings r = new ReminderSettings { TimeOfDay = t };

            Assert.AreEqual(t, r.TimeOfDay);
        }

        [TestMethod]
        public void Test_ReminderSettings_EnabledDefaultIsFalse() {
            ReminderSettings r = new ReminderSettings();
            Assert.IsFalse(r.Enabled);
        }
    }

    [TestClass]
    public class ChallengeTests {
        [TestMethod]
        public void Test_Challenge_DefaultCompletedFalse() {
            Challenge c = new Challenge();
            Assert.IsFalse(c.Completed);
        }

        [TestMethod]
        public void Test_Challenge_StoresDescription() {
            Challenge c = new Challenge { Description = "Read 5 books" };
            Assert.AreEqual("Read 5 books", c.Description);
        }

        [TestMethod]
        public void Test_Challenge_AssignsUserId() {
            Guid id = Guid.NewGuid();
            Challenge c = new Challenge { UserId = id };

            Assert.AreEqual(id, c.UserId);
        }
    }

    [TestClass]
    public class MilestoneNotificationTests {
        [TestMethod]
        public void Test_Milestone_HasCreatedAtDefault() {
            MilestoneNotification m = new MilestoneNotification();
            Assert.IsTrue(m.CreatedAt <= DateTime.UtcNow);
        }

        [TestMethod]
        public void Test_Milestone_StoresMessage() {
            MilestoneNotification m = new MilestoneNotification {
                Message = "Reached 10 books!"
            };

            Assert.AreEqual("Reached 10 books!", m.Message);
        }

        [TestMethod]
        public void Test_Milestone_AssignsUserId() {
            Guid id = Guid.NewGuid();
            MilestoneNotification m = new MilestoneNotification { UserId = id };

            Assert.AreEqual(id, m.UserId);
        }
    }

    [TestClass]
    public class RecommendationTests {
        [TestMethod]
        public void Test_Recommendation_AssignsBookId() {
            Guid id = Guid.NewGuid();
            Recommendation r = new Recommendation { BookId = id };

            Assert.AreEqual(id, r.BookId);
        }

        [TestMethod]
        public void Test_Recommendation_AssignsUsers() {
            Guid from = Guid.NewGuid();
            Guid to = Guid.NewGuid();

            Recommendation r = new Recommendation {
                FromUserId = from,
                ToUserId = to
            };

            Assert.AreEqual(from, r.FromUserId);
            Assert.AreEqual(to, r.ToUserId);
        }

        [TestMethod]
        public void Test_Recommendation_StoresNote() {
            Recommendation r = new Recommendation { Note = "Great book!" };

            Assert.AreEqual("Great book!", r.Note);
        }
    }

    [TestClass]
    public class CommentTests {
        [TestMethod]
        public void Test_Comment_DefaultTimestampSet() {
            Comment c = new Comment();
            Assert.IsTrue(c.CreatedAt <= DateTime.UtcNow);
        }

        [TestMethod]
        public void Test_Comment_TextStored() {
            Comment c = new Comment { Text = "Nice read!" };
            Assert.AreEqual("Nice read!", c.Text);
        }

        [TestMethod]
        public void Test_Comment_AssignsIds() {
            Guid book = Guid.NewGuid();
            Guid user = Guid.NewGuid();

            Comment c = new Comment {
                BookId = book,
                AuthorId = user
            };

            Assert.AreEqual(book, c.BookId);
            Assert.AreEqual(user, c.AuthorId);
        }
    }

    [TestClass]
    public class ThemeSettingsTests {
        [TestMethod]
        public void Test_ThemeSettings_DefaultModeLight() {
            ThemeSettings t = new ThemeSettings();
            Assert.AreEqual(ThemeMode.Light, t.Mode);
        }

        [TestMethod]
        public void Test_ThemeSettings_StoresPrimaryColor() {
            ThemeSettings t = new ThemeSettings { PrimaryColor = "#FF00FF" };
            Assert.AreEqual("#FF00FF", t.PrimaryColor);
        }

        [TestMethod]
        public void Test_ThemeSettings_AssignsUserId() {
            Guid id = Guid.NewGuid();
            ThemeSettings t = new ThemeSettings { UserId = id };

            Assert.AreEqual(id, t.UserId);
        }
    }

    [TestClass]
    public class ReadingProfileTests {
        [TestMethod]
        public void Test_ReadingProfile_DefaultUpdatedAtSet() {
            ReadingProfile p = new ReadingProfile();
            Assert.IsTrue(p.UpdatedAt <= DateTime.UtcNow);
        }

        [TestMethod]
        public void Test_ReadingProfile_StoresSummary() {
            ReadingProfile p = new ReadingProfile {
                Summary = "Avid reader of fantasy."
            };

            Assert.AreEqual("Avid reader of fantasy.", p.Summary);
        }

        [TestMethod]
        public void Test_ReadingProfile_AssignsUserId() {
            Guid id = Guid.NewGuid();
            ReadingProfile p = new ReadingProfile { UserId = id };

            Assert.AreEqual(id, p.UserId);
        }
    }


}
