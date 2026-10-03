using NUnit.Framework;

namespace Chaldran.Tests
{
    public sealed class RetroPresentationTests
    {
        [Test]
        public void TextRevealsQuicklyAndStopsAtThePageEnd()
        {
            DialogueReveal reveal = new DialogueReveal();
            reveal.Begin(120, 60);
            reveal.Tick(0.25f);
            Assert.That(reveal.VisibleCharacters, Is.EqualTo(15));
            reveal.Tick(10);
            Assert.That(reveal.VisibleCharacters, Is.EqualTo(120));
            Assert.That(reveal.Complete, Is.True);
        }

        [Test]
        public void RevealRateDoesNotDependOnFrameSize()
        {
            DialogueReveal many = new DialogueReveal(), one = new DialogueReveal();
            many.Begin(120, 60); one.Begin(120, 60);
            for (int i = 0; i < 10; i++) many.Tick(0.025f);
            one.Tick(0.25f);
            Assert.That(many.VisibleCharacters, Is.EqualTo(one.VisibleCharacters));
        }

        [Test]
        public void RevealIgnoresInvalidTimeAndResetsForEachPage()
        {
            DialogueReveal reveal = new DialogueReveal();
            reveal.Begin(100, 60);
            reveal.Tick(float.NaN); reveal.Tick(float.PositiveInfinity); reveal.Tick(-1);
            Assert.That(reveal.VisibleCharacters, Is.Zero);
            reveal.RevealAll();
            reveal.Begin(50, 60);
            Assert.That(reveal.VisibleCharacters, Is.Zero);
            reveal.Tick(0.5f);
            Assert.That(reveal.VisibleCharacters, Is.EqualTo(30));
        }

        [Test]
        public void FirstConfirmRevealsWithoutSkippingOrCompletingThePage()
        {
            DialogueSession session = new DialogueSession();
            DialogueReveal reveal = new DialogueReveal();
            session.Begin(2); reveal.Begin(120, 60);
            Assert.That(session.ConfirmPage(reveal), Is.EqualTo(DialogueConfirm.Revealed));
            Assert.That(session.LineIndex, Is.Zero);
            Assert.That(session.IsOpen, Is.True);
            Assert.That(reveal.Complete, Is.True);
            Assert.That(session.ConfirmPage(reveal), Is.EqualTo(DialogueConfirm.NextPage));
            reveal.Begin(30, 60);
            Assert.That(session.ConfirmPage(reveal), Is.EqualTo(DialogueConfirm.Revealed));
            Assert.That(session.ConfirmPage(reveal), Is.EqualTo(DialogueConfirm.Completed));
            Assert.That(session.ConfirmPage(reveal), Is.EqualTo(DialogueConfirm.None));
        }

        [Test]
        public void InstantModeAndEmptyTextHaveNoForcedWait()
        {
            DialogueReveal reveal = new DialogueReveal();
            reveal.Begin(100, 0);
            Assert.That(reveal.Complete, Is.True);
            reveal.Begin(100, float.NaN);
            Assert.That(reveal.Complete, Is.True);
            reveal.Begin(0, 60);
            Assert.That(reveal.Complete, Is.True);
        }

        [Test]
        public void CancelAfterFastForwardDoesNotAwardCompletion()
        {
            DialogueSession session = new DialogueSession();
            DialogueReveal reveal = new DialogueReveal();
            session.Begin(1); reveal.Begin(100, 60);
            Assert.That(session.ConfirmPage(reveal), Is.EqualTo(DialogueConfirm.Revealed));
            session.Cancel();
            Assert.That(session.ConfirmPage(reveal), Is.EqualTo(DialogueConfirm.None));
        }

        [Test]
        public void GuardRouteTravelsAroundAnObstacleWithCardinalSteps()
        {
            var route = SentinelRoute.Find(0, 1, 0, 4, 0, 2, (x,y) => x != 2 || y == 0, (x,y) => x == 4 && y == 1);
            Assert.That(route, Is.Not.Null);
            GridPoint previous = new GridPoint(0,1);
            foreach (GridPoint point in route)
            {
                Assert.That(point.X != 2 || point.Y == 0, Is.True);
                Assert.That(System.Math.Abs(point.X - previous.X) + System.Math.Abs(point.Y - previous.Y), Is.EqualTo(1));
                previous = point;
            }
            Assert.That(previous.X, Is.EqualTo(4));
            Assert.That(previous.Y, Is.EqualTo(1));
        }

        [Test]
        public void GuardCannotCutADiagonalThroughBlockedCorners()
        {
            var route = SentinelRoute.Find(0, 0, 0, 1, 0, 1, (x,y) => x == y, (x,y) => x == 1 && y == 1);
            Assert.That(route, Is.Null);
            Assert.That(SentinelRoute.Find(-1,0,0,1,0,1,(x,y)=>true,(x,y)=>true), Is.Null);
        }

        [Test]
        public void GuardSearchTerminatesWhenASealedBarrierBlocksTheTarget()
        {
            var route = SentinelRoute.Find(0, 1, 0, 4, 0, 2, (x,y) => x != 2, (x,y) => x == 4);
            Assert.That(route, Is.Null);
        }
    }
}
