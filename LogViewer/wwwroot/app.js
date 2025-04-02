$(document).ready(function () {
    // Global variables to track current selections
    let currentDirectory = null;
    let currentFile = null;

    // Initialize the application
    loadDirectories();

    // Event handlers
    $('#refreshLog').click(function () {
        if (currentFile) {
            loadLogContent(currentFile);
        }
    });

    $('#linesCount').change(function () {
        if (currentFile) {
            loadLogContent(currentFile);
        }
    });

    // Function to load directories from the API
    function loadDirectories() {
        $('#dirSpinner').show();
        $('#directoryList').empty();

        $.get('/api/log/directories')
            .done(function (directories) {
                if (directories && directories.length > 0) {
                    directories.forEach(function (dir) {
                        const dirElement = $(`
                            <div class="directory-item mb-1 p-2 rounded cursor-pointer" data-path="${dir}">
                                <i class="bi bi-folder"></i> ${dir}
                            </div>
                        `);

                        dirElement.click(function () {
                            $('.directory-item').removeClass('active-directory');
                            dirElement.addClass('active-directory');
                            currentDirectory = dir;
                            loadFiles(dir);
                        });

                        $('#directoryList').append(dirElement);
                    });

                    // Auto-select the first directory
                    if (directories.length > 0) {
                        $('.directory-item').first().click();
                    }
                } else {
                    $('#directoryList').html('<p class="text-muted">No log directories configured</p>');
                }
            })
            .fail(function () {
                $('#directoryList').html('<p class="text-danger">Failed to load directories</p>');
            })
            .always(function () {
                $('#dirSpinner').hide();
            });
    }

    // Function to load files from a directory
    function loadFiles(directoryPath) {
        $('#fileList').html('<div class="text-center"><div class="spinner-border spinner-border-sm" role="status"><span class="visually-hidden">Loading...</span></div></div>');

        $.get('/api/log/files', { directoryPath: directoryPath })
            .done(function (files) {
                $('#fileList').empty();

                if (files && files.length > 0) {
                    files.forEach(function (file) {
                        const fileElement = $(`
                            <div class="file-item mb-1 p-2 rounded cursor-pointer" data-path="${file.path}">
                                <i class="bi bi-file-earmark-text"></i> ${file.name}
                                <small class="text-muted float-end">${formatDate(file.lastModified)}</small>
                            </div>
                        `);

                        fileElement.click(function () {
                            $('.file-item').removeClass('active-file');
                            fileElement.addClass('active-file');
                            currentFile = file.path;
                            $('#currentFile').text(file.path);
                            loadLogContent(file.path);
                        });

                        $('#fileList').append(fileElement);
                    });

                    // Auto-select the first file
                    if (files.length > 0) {
                        $('.file-item').first().click();
                    }
                } else {
                    $('#fileList').html('<p class="text-muted">No log files found in this directory</p>');
                }
            })
            .fail(function () {
                $('#fileList').html('<p class="text-danger">Failed to load files</p>');
            });
    }

    // Function to load log content
    function loadLogContent(filePath) {
        const lines = $('#linesCount').val();
        $('#logContent').html('<div class="text-center"><div class="spinner-border spinner-border-sm text-light" role="status"><span class="visually-hidden">Loading...</span></div></div>');

        $.get('/api/log/content', { filePath: filePath, lines: lines })
            .done(function (data) {
                $('#logContent').text(data.content);
            })
            .fail(function () {
                $('#logContent').text('Failed to load log content');
            });
    }

    // Helper function to format date
    function formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleString();
    }
});