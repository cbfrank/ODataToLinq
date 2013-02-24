namespace ODataToLinq.Test
{
    using ExpressionParsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class FilterExpressionParserTest
    {
        private class Dummy
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Active { get; set; }
        }

        private readonly List<Dummy> _dummies = new List<Dummy>
        #region dummy data
            {
                new Dummy
                {
                    Id = 1,
                    Name = "Aa",
                    Active = false
                },
                new Dummy
                {
                    Id = 2,
                    Name = "Bb",
                    Active = true

                },
                new Dummy
                {
                    Id = 3,
                    Name = "Cc",
                    Active = false

                }
            };
        #endregion

        [TestMethod]
        public void FilterExpressionParser_BoolTrueEqual_Success()
        {
            // Prepare
            var expected = _dummies.Single(d => d.Active);

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Active eq true)").ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void FilterExpressionParser_BoolTrueNotEqual_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => !d.Active).ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Active ne true)").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_BoolFalseEqual_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => !d.Active).ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Active eq false)").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_BoolFalseNotEqual_Success()
        {
            // Prepare
            var expected = _dummies.Single(d => d.Active);

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Active ne false)").ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void FilterExpressionParser_StringEqual_Success()
        {
            // Prepare
            var expected = _dummies.Single(d => d.Name == "Bb");

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Name eq 'Bb')").ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void FilterExpressionParser_StringNotEqual_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => d.Name != "Bb").ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Name ne 'Bb')").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_StringWithSpaceEqual_Success()
        {
            // Prepare

            // Act
            //var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Name eq 'B b')").ToList();

            // Assert
            //Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        public void FilterExpressionParser_IntEqual_Success()
        {
            // Prepare
            var expected = _dummies.Single(d => d.Id == 2);

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Id eq 2)").ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void FilterExpressionParser_IntNotEqual_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => d.Id != 2).ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Id ne 2)").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_IntGreater_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => d.Id > 1).ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Id gt 1)").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_IntGreaterOrEqual_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => d.Id >= 2).ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Id ge 2)").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_IntLess_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => d.Id < 3).ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Id lt 3)").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_IntLessOrEqual_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => d.Id <= 2).ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Id le 2)").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_And_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => d.Id == 2 && d.Name == "Bb").ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Id eq 2) and (Name eq 'Bb')").ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
        }

        [TestMethod]
        public void FilterExpressionParser_Or_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => d.Id == 1 || d.Name == "Bb").ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Id eq 1) or (Name eq 'Bb')").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_AndOr_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => (d.Id == 1 && d.Name == "Aa") || d.Name == "Bb").ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "((Id eq 1) and (Name eq 'Aa')) or (Name eq 'Bb')").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_OrAnd_Success()
        {
            // Prepare
            var expected = _dummies.Where(d => d.Name == "Bb" || (d.Id == 1 && d.Name == "Aa")).ToList();

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Name eq 'Bb') or ((Id eq 1) and (Name eq 'Aa'))").ToList();

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(expected[0], actual[0]);
            Assert.AreEqual(expected[1], actual[1]);
        }

        [TestMethod]
        public void FilterExpressionParser_StartsWith_Success()
        {
            // Prepare
            var expected = _dummies.Single(d => d.Name.StartsWith("B"));

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(startswith(Name,'B') eq true)").ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void FilterExpressionParser_EndsWith_Success()
        {
            // Prepare
            var expected = _dummies.Single(d => d.Name.EndsWith("b"));

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(endswith(Name,'b') eq true)").ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void FilterExpressionParser_Contains_Success()
        {
            // Prepare
            var expected = _dummies.Single(d => d.Name.Contains("B"));

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(substringof(Name,'B') eq true)").ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected, actual[0]);
        }


        [TestMethod]
        public void DummyTest()
        {
            // Prepare
            var expected = _dummies.Single(d => d.Name == "Bb");

            // Act
            var actual = FilterExpressionParser.Parse(_dummies.AsQueryable(), "(Name eq 'B b')").ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected, actual[0]);
        }
    
    }
}
