﻿Imports System.Threading

Public Class Form1

    Private sc As ServerController
    Private worker As Thread
    Private nf As Integer = 0

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        logger.debug("Try to connect casparCG server..")
        If IsNothing(sc) Then
            sc = New ServerController()
        End If
        If Not sc.isOpen() Then
            sc.open("casparcg", 5250)
        End If

        'Thread.Sleep(1000)

        'doIt()

        testPlaylist()

    End Sub

    Private Sub testPlaylist()
        Dim mediaLib As New Library(sc)
        mediaLib.refreshLibrary()

        Dim p As New PlaylistBlockItem("GESAMT LISTE", sc)
        Dim p1 As New PlaylistBlockItem("P1_SEQ", sc)
        Dim p2 As New PlaylistBlockItem("P2_SEQ", sc)

        p1.setChannel(1)
        p2.setChannel(1)
        p1.setParallel(False)
        p2.setParallel(True)
        p1.setAutoStart(True)
        p2.setAutoStart(True)

        p1.addItem(New PlaylistMovieItem("amb", sc, mediaLib.getItem("amb")))
        p1.addItem(New PlaylistMovieItem("cg1080i50", sc, New CasparCGMovie("cg1080i50")))

        p2.addItem(New PlaylistMovieItem("go1080p25", sc, mediaLib.getItem("go1080p25")))
        p2.addItem(New PlaylistMovieItem("cg1080i50", sc, mediaLib.getItem("cg1080i50"), , 2))

        p.addItem(p1)
        p.addItem(p2)
        sc.getPlaylistRoot.addItem(p)
        sc.getPlaylistRoot.start()

    End Sub

    Private Sub openListener()
        Dim f2 As Form2
        For i = nf To Integer.Parse(txtListenerCount.Text) - 1
            nf = nf + 1
            f2 = New Form2(sc)
            f2.Text = nf
            f2.Show()
        Next
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'sc.toggleTickerActive()
        worker = New Thread(AddressOf doIt)
        worker.Start()
    End Sub

    Private Sub doIt()
        Dim mediaLib As New Library(sc)
        logger.log("Refreshing Library")
        mediaLib.refreshLibrary()
        logger.log("LIBRARY:")
        For Each media As CasparCGMedia In mediaLib.getItems
            logger.log(media.toString)
        Next
    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        logger.addLogAction(New consoleLogger(3))
        logger.log("Console Logger startet")
    End Sub

    Private Sub Form2_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        sc.close()
        MyBase.Finalize()
    End Sub
End Class
