using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using FluentAssertions;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.PowerCalc.ViewModels.Worksheet;


namespace TAlex.PowerCalc.Tests.ViewModels.Worksheet
{
    [TestFixture]
    public class WorksheetModelTests
    {
        protected WorksheetModel ViewModel;
        protected IExpressionTreeBuilder<Object> TreeBuilderMock;


        [SetUp]
        public virtual void SetUp()
        {
            TreeBuilderMock = Substitute.For<IExpressionTreeBuilder<Object>>();

            ViewModel = new WorksheetModel(TreeBuilderMock);
        }


        #region NavigateUpInputInputHistory Tests

        [Test]
        public void NavigateUpInputHistory_Empty()
        {
            //action
            ViewModel.NavigateUpInputHistory();

            //assert
            ViewModel.EditableItem.Expression.Should().BeEmpty();
        }

        [Test]
        public void NavigateUpInputHistory_1Item()
        {
            //arrange
            const string expr = "1";
            ViewModel.EditableItem.Expression = expr;
            ViewModel.Evaluate();

            //action
            ViewModel.NavigateUpInputHistory();

            //assert
            ViewModel.EditableItem.Expression.Should().Be(expr);
        }

        [Test]
        public void NavigateUpInputHistory_2Items()
        {
            //arrange
            const string expr1 = "1";
            ViewModel.EditableItem.Expression = expr1;
            ViewModel.Evaluate();
            const string expr2 = "2";
            ViewModel.EditableItem.Expression = expr2;
            ViewModel.Evaluate();

            //assert
            ViewModel.NavigateUpInputHistory();
            ViewModel.EditableItem.Expression.Should().Be(expr2);
            ViewModel.NavigateUpInputHistory();
            ViewModel.EditableItem.Expression.Should().Be(expr1);
        }

        [Test]
        public void NavigateUpInputHistory_IgnoreDuplicates()
        {
            //arrange
            const string expr1 = "1";
            ViewModel.EditableItem.Expression = expr1;
            ViewModel.Evaluate();
            ViewModel.EditableItem.Expression = expr1;
            ViewModel.Evaluate();
            const string expr2 = "2";
            ViewModel.EditableItem.Expression = expr2;
            ViewModel.Evaluate();
            ViewModel.EditableItem.Expression = expr2;
            ViewModel.Evaluate();

            //assert
            ViewModel.NavigateUpInputHistory();
            ViewModel.EditableItem.Expression.Should().Be(expr2);
            ViewModel.NavigateUpInputHistory();
            ViewModel.EditableItem.Expression.Should().Be(expr1);
        }

        [Test]
        public void NavigateUpInputHistory_IgnoreDuplicates2()
        {
            //arrange
            const string expr1 = "1";
            ViewModel.EditableItem.Expression = expr1;
            ViewModel.Evaluate();
            ViewModel.EditableItem.Expression = expr1;
            ViewModel.Evaluate();
            const string expr2 = "2";
            ViewModel.EditableItem.Expression = expr2;
            ViewModel.Evaluate();
            ViewModel.EditableItem.Expression = expr2;
            ViewModel.Evaluate();
            ViewModel.NavigateUpInputHistory();
            ViewModel.Evaluate();

            //assert
            ViewModel.NavigateUpInputHistory();
            ViewModel.EditableItem.Expression.Should().Be(expr2);
            ViewModel.NavigateUpInputHistory();
            ViewModel.EditableItem.Expression.Should().Be(expr1);
        }

        [Test]
        public void NavigateUpInputHistory_SpecialCase()
        {
            //arrange
            const string expr1 = "1";
            ViewModel.EditableItem.Expression = expr1;
            ViewModel.Evaluate();
            const string expr2 = "2";
            ViewModel.EditableItem.Expression = expr2;
            ViewModel.Evaluate();
            ViewModel.NavigateUpInputHistory();
            ViewModel.NavigateUpInputHistory();
            ViewModel.Evaluate();

            //assert
            ViewModel.NavigateDownInputHistory();
            ViewModel.EditableItem.Expression.Should().Be(expr2);
            ViewModel.NavigateDownInputHistory();
            ViewModel.EditableItem.Expression.Should().Be(expr1);
        }

        #endregion

        #region NavigateDownInputHistory Tests

        [Test]
        public void NavigateDownInputHistory_Empty()
        {
            //action
            ViewModel.NavigateDownInputHistory();

            //assert
            ViewModel.EditableItem.Expression.Should().BeEmpty();
        }

        [Test]
        public void NavigateDownInputHistory_1Item()
        {
            //arrange
            const string expr = "1";
            ViewModel.EditableItem.Expression = expr;
            ViewModel.Evaluate();

            //action
            ViewModel.NavigateDownInputHistory();

            //assert
            ViewModel.EditableItem.Expression.Should().BeEmpty();
        }

        #endregion
    }
}
